using Convergence.Interfaces;
using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.EPICS.ConnectionCallback;
using Convergence.IO.EPICS;

namespace Convergence
{
    public class ConvergenceInstance : IConvergence
    {
        // Singleton instance of Convergence.
        private static ConvergenceInstance _hub;

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
        
        public EndPointID Connect<T>(EndPointID id, EndPointBase<T> endPointArgs)
        {
            switch (id.Protocol)
            {
                case Protocols.EPICS:
                    IntPtr epicsHandle;
                    // Always call ca_context_create() before any other Channel Access calls
                    EPICSWrapper.ca_context_create(PreemptiveCallbacks.DISABLE);
                    switch (EPICSWrapper.ca_create_channel(id.EndPointName, null, out epicsHandle))
                    {
                        case EcaType.ECA_NORMAL:
                            return new EndPointID(Protocols.EPICS, new Guid());
                            break;
                    }
                    break;
            }
            return EndPointID.Empty;
        }
    }
}