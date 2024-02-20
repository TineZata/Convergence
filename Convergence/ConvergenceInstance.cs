﻿using Convergence.Interfaces;
using Convergence.IO;
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

        /// <summary>
        /// Disconnects the EndPointID from the network.
        /// </summary>
        /// <param name="endPointID"></param>
        public void Disconnect(EndPointID endPointID)
        {
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    EpicsCaDisconnect(endPointID);
                    break;
            }
        }

        public async Task<EndPointStatus> ReadAsync(EndPointID endPointID, ValueUpdateCallback? callback)
        {
            EndPointStatus status =  EndPointStatus.Disconnected;
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    status = await EpicsCaReadAsync(endPointID, callback);
                    break;
            }
            return status;
        }

        
    }
}