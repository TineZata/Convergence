using Convergence.Interfaces;
using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.EPICS.ConnectionCallback;
using Convergence.IO.EPICS;
using System.Collections.Concurrent;

namespace Convergence
{
    public class ConvergenceInstance : IConvergence
    {
        // Singleton instance of Convergence.
        private static ConvergenceInstance? _hub;

        // ConcurrentDictionary of all the EPICS CA connections as a key-value pair of EndPointID and EndPointBase<EPICSSettings>.
        private static ConcurrentDictionary<EndPointID, Convergence.IO.EPICS.Settings>? _epics_ca_connections = new();

        /// <summary>
        /// Network communication hub, which keeps track of all the connections on all protocols.
        /// 
        /// Must be a singleton because network communication is an expensive resource and we don't want to have multiple instances of it.
        /// </summary>
        public static ConvergenceInstance Hub
        {
            get
            {
                if (_hub == null)
                {
                    _hub = new ConvergenceInstance();
                }
                return _hub;
            }
        }
        // Private constructor for singleton, to prevent external instantiation.
        private ConvergenceInstance() { }
        
        public EndPointID Connect<T>(EndPointBase<T> endPointArgs)
        {
            switch (endPointArgs.Id.Protocol)
            {
                case Protocols.EPICS_CA:
                    var epicsSettings = endPointArgs.ConvertToEPICSSettings();
                    // Always call ca_context_create() before any other Channel Access calls from the thread you want to use Channel Access from.
                    EPICSWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
                    switch (EPICSWrapper.ca_create_channel(endPointArgs.Id.EndPointName ?? "", null, out epicsSettings.ChannelHandle))
                    {
                        case EcaType.ECA_NORMAL:
                            endPointArgs.Id.UniqueId = Guid.NewGuid();
                            _epics_ca_connections!.TryAdd(endPointArgs.Id, epicsSettings);
                            break;
                        case EcaType.ECA_BADSTR:
                            throw new ArgumentException("Invalid channel name");
                            break;
                    }
                    break;
            }
            return endPointArgs.Id;
        }
    }
}