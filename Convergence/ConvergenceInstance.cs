using Convergence.Interfaces;
using Convergence.IO;
using Convergence.IO.EPICS;
using System.Collections.Concurrent;
using static Convergence.IO.EPICS.EventCallbackDelegate;

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

        /// <summary>
        /// Asynchronous read method for all protocols.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="readCallback"></param>
        /// <returns></returns>
        public async Task<EndPointStatus> ReadAsync(EndPointID endPointID, ReadCallback? readCallback)
        {
            EndPointStatus status = EndPointStatus.UnknownError;
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    var result = await EpicsCaReadAsync(endPointID, readCallback);
                    switch (result)
                    {
                        case EcaType.ECA_NORMAL:
                            status = EndPointStatus.Okay;
                            break;
                        case EcaType.ECA_BADTYPE:
                            status = EndPointStatus.InvalidDataType;
                            break;
                        case EcaType.ECA_NORDACCESS:
                            status = EndPointStatus.NoReadAccess;
                            break;
                        case EcaType.ECA_DISCONN:
                            status = EndPointStatus.Disconnected;
                            break;
                        case EcaType.ECA_UNAVAILINSERV:
                            status = EndPointStatus.Disconnected;
                            break;
                        case EcaType.ECA_TIMEOUT:
                            status = EndPointStatus.TimedOut;
                            break;
                        case EcaType.ECA_BADCOUNT:
                        case EcaType.ECA_ALLOCMEM:
                        case EcaType.ECA_TOLARGE:
                        case EcaType.ECA_GETFAIL:
                        default:
                            status = EndPointStatus.UnknownError;
                            break;
                    }
                    break;
            }
            return status;
        } 

        /// <summary>
        /// Asynchronous write method for all protocols.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<EndPointStatus> WriteAsync(EndPointID endPointID, IntPtr value, WriteCallback? callback)
        {
            EndPointStatus status = EndPointStatus.UnknownError;
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    var result = await EpicsCaWriteAsync(endPointID, value, callback);
                    switch (result)
                    {
                        case EcaType.ECA_NORMAL:
                            status = EndPointStatus.Okay;
                            break;
                        case EcaType.ECA_BADTYPE:
                            status = EndPointStatus.InvalidDataType;
                            break;
                        case EcaType.ECA_NOWTACCESS:
                            status = EndPointStatus.NoWriteAccess;
                            break;
                        case EcaType.ECA_DISCONN:
                            status = EndPointStatus.Disconnected;
                            break;
                        case EcaType.ECA_UNAVAILINSERV:
                            status = EndPointStatus.Disconnected;
                            break;
                        case EcaType.ECA_TIMEOUT:
                            status = EndPointStatus.TimedOut;
                            break;
                        case EcaType.ECA_BADCOUNT:
                        case EcaType.ECA_ALLOCMEM:
                        case EcaType.ECA_TOLARGE:
                        case EcaType.ECA_PUTFAIL:
                        default:
                            status = EndPointStatus.UnknownError;
                            break;
                    }
                    break;
            }
            return status;
        }
    }
}