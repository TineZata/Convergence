using Convergence.Interfaces;
using Convergence.IO;
using Convergence.IO.EPICS;
using Conversion.IO.EPICS;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using static Convergence.EventCallbackDelegate;
using static Convergence.IO.EPICS.CaEventCallbackDelegate;

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
          get => _hub ??= new ConvergenceInstance();
        }
        // Private constructor for singleton, to prevent external instantiation.
        private ConvergenceInstance() { }
        
        /// <summary>
        /// Generic Connect method for all protocols.
        /// 
        /// Callback will not be call is the initial connection fails. Callback indicated a change in connection and
        /// therefore if no connection is made in the first place, then there is no change to the connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointArgs"></param>
        /// <param name="connectCallback"></param>
        /// <returns></returns>
        public async Task<EndPointStatus> ConnectAsync<T1,T2>(EndPointBase<T1> endPointArgs, T2? connectCallback)
        {
            switch (endPointArgs.EndPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    if (endPointArgs is EndPointBase<Convergence.IO.EPICS.Settings>)
                    {
                        var settings = endPointArgs.Settings as Convergence.IO.EPICS.Settings;
                        var callback = connectCallback as CaConnectCallback;    
                        var result = await EpicsCaConnectAsync(endPointArgs.EndPointID, settings, callback);
                        // Introduce a delay if callback is not null as pend_io will not block.
                        if (callback != null)
                        {
                            await Task.Delay((int)(ConvergenceInstance.EPICS_TIMEOUT_SEC*10000));
                        }
                        return GetEPICSEndPointStatus(result);
                    }
                    break;
            }
            return EndPointStatus.UnknownError;
        }

        /// <summary>
        /// Disconnects the EndPointID from the network.
        /// </summary>
        /// <param name="endPointID"></param>
        public bool Disconnect(EndPointID endPointID)
        {
            bool disconnected = false;
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    disconnected = (EpicsCaDisconnect(endPointID) == EcaType.ECA_NORMAL);
                    break;
            }
            return disconnected;
        }

        /// <summary>
        /// Asynchronous read method for all protocols.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="readCallback"></param>
        /// <returns></returns>
        public async Task<EndPointStatus> ReadAsync<T>(EndPointID endPointID, T? readCallback)
        {
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    var result = await EpicsCaReadAsync(endPointID, readCallback as CaReadCallback);
                    return GetEPICSEndPointStatus(result);
            }
            return EndPointStatus.UnknownError;
        }

        /// <summary>
        /// Asynchronous write method for all protocols.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="value"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<EndPointStatus> WriteAsync<T>(EndPointID endPointID, IntPtr value, T? callback)
        {
            switch (endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    var result = await EpicsCaWriteAsync(endPointID, value, callback as CaWriteCallback);
                    return GetEPICSEndPointStatus(result);
            }
            return EndPointStatus.UnknownError;
        }

        /// <summary>
        /// Ayncronous subscribe method for all protocols.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="endPointID"></param>
        /// <param name="monitorType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<EndPointStatus> SubscribeAsync<T1, T2>(EndPointID endPointID, T1 monitorType, T2 callback)
        {
            switch(endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
                    var result = await EpicsCaMonitor(endPointID, monitorType as CaMonitorTypes?, callback as CaMonitorCallback);
                    return GetEPICSEndPointStatus(result);
            }
            return EndPointStatus.UnknownError;
        }
    }
}