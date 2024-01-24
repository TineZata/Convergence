using Convergence.Interfaces;
using Convergence.IO.EPICS;
using System.Collections.Concurrent;

namespace Convergence
{
    public partial class ConvergenceInstance : IConvergence
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
        
        /// <summary>
        /// Generic Connect method for all protocols.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointArgs"></param>
        /// <returns></returns>
        public void Connect<T>(EndPointBase<T> endPointArgs)
        {
            switch (endPointArgs.EndPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    EpicsCaConnect(endPointArgs);
                    break;
            }
            
        }

        public void Disconnect(EndPointID endPointID)
        {
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    EpicsCaDisconnect(endPointID);
                    break;
            }
        }

        
    }
}