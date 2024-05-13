using Convergence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Convergence.IO.EPICS.CA;
using static Convergence.IO.EPICS.CA.EventCallbackDelegate;
using System.Net;
using System.Threading;

namespace Convergence.IO.EPICS.CA
{
    public class ConvergeOnEPICSChannelAccess : IConvergence
    {
        public static readonly double EPICS_TIMEOUT_SEC = 0.5;

        /// <summary>
        /// Singleton instance of Convergence on EPICS Channel Access.
        /// </summary>
        private static ConvergeOnEPICSChannelAccess? _hub;
        /// <summary>
        /// ConcurrentDictionary of all the EPICS CA connections as a key-value pair of EndPointID and EndPointBase<EPICSSettings>.
        /// </summary>
        private static Dictionary<EndPointID, Settings>? _epics_ca_connections = new();

        /// <summary>
        /// Network communication hub, which keeps track of all the connections on all EPICS Channels Access endpoints.
        /// </summary>
        public static ConvergeOnEPICSChannelAccess Hub
        {
            get => _hub ??= new ConvergeOnEPICSChannelAccess();
        }
        /// <summary>
        /// Private constructor for singleton, to prevent external instantiation.
        /// </summary>
        private ConvergeOnEPICSChannelAccess()
        {
        }

        /// <summary>
        /// Handles the EPICS CA connection.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="endPointArgs"> As EndPointBase<Convergence.IO.EPICS.CA.Settings></param>
        /// <param name="connectCallback">As Convergence.IO.EPICS.CA.CaEventCallbackDelegate.CaConnectCallback</param>
        /// <returns></returns>
        public Task<EndPointStatus> ConnectAsync<T1, T2>(EndPointBase<T1> endPointArgs, T2? connectCallback)
        {
            var settings = endPointArgs.Settings as Settings;
            var callback = connectCallback as ConnectCallback;
            if (settings == null)
            {
                return Task.FromResult(EndPointStatus.InvaildProtocol);
            }
            else
            {
                var tcs = new TaskCompletionSource<EcaType>();
                // Check if the CA ID already exists.
                if (_epics_ca_connections!.ContainsKey(endPointArgs.EndPointID))
                {
                    settings = _epics_ca_connections[endPointArgs.EndPointID];
                    tcs.SetResult(EcaType.ECA_NORMAL);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }
                else
                {
                    // Always call ca_context_create() before any other Channel Access calls from the thread you want to use Channel Access from.
                    var contextResult = ChannelAccessWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
                    if (contextResult != EcaType.ECA_NORMAL)
                    {
                        tcs.SetResult(contextResult);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }

                    //var chCreateResult = ChannelAccessDLLWrapper.ca_create_channel(endPointArgs.EndPointID.EndPointName ?? "", null, out epicsSettings.ChannelHandle);
                    // Do connection with a connection callback.
                    EcaType chCreateResult = EcaType.ECA_DISCONN;
                    chCreateResult = ChannelAccessWrapper.ca_create_channel(
                                           endPointArgs.EndPointID.EndPointName ?? "",
                                            callback,
                                            out settings.ChannelHandle);
                    if (chCreateResult != EcaType.ECA_NORMAL)
                    {
                        tcs.SetResult(chCreateResult);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }

                    // If the callback is not null the channel access does not block on a pend_io, 
                    // however a call is still required to flush the IO.
                    if (ChannelAccessWrapper.ca_pend_io(EPICS_TIMEOUT_SEC) == EcaType.ECA_EVDISALLOW)
                    {
                        tcs.SetResult(EcaType.ECA_EVDISALLOW);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }
                    // Introduce an artificial delay if callback is not null.
                    if (connectCallback != null)
                        Thread.Sleep((int)(EPICS_TIMEOUT_SEC * 1000));
                    // If the callback is null, then we need to explicitly check the state of the channel,
                    // as connection will just return ECA_NORMAL, so an additional check is required.
                    if (connectCallback == null)
                    {
                        var state = ChannelAccessWrapper.ca_state(settings.ChannelHandle);
                        if (state == ChannelState.NeverConnected || state == ChannelState.Closed)
                        {
                            tcs.SetResult(EcaType.ECA_DISCONN);
                            return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                        }
                    }

                    if (chCreateResult == EcaType.ECA_NORMAL)
                    {
                        // Try add a new ID, if not already added.
                        endPointArgs.EndPointID.UniqueId = Guid.NewGuid();
                        if (!_epics_ca_connections!.TryAdd(endPointArgs.EndPointID, settings))
                            tcs.SetResult(EcaType.ECA_NEWCONN); // New or resumed network connection
                        else
                            tcs.SetResult(EcaType.ECA_NORMAL);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }
                    else
                    {
                        tcs.SetResult(chCreateResult);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }
                }

            }
        }

        /// <summary>
        /// Handles the EPICS CA disconnection.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <returns></returns>
        public bool Disconnect(EndPointID endPointID)
        {
            bool disconnected = false;
            EcaType result = EcaType.ECA_NOSUPPORT;
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                result = ChannelAccessWrapper.ca_clear_channel(_epics_ca_connections[endPointID].ChannelHandle);
                switch (result)
                {
                    case EcaType.ECA_NORMAL:
                        disconnected = _epics_ca_connections.Remove(endPointID, out _);
                        break;
                }
            }
            return disconnected;
        }

        /// <summary>
        /// Handles the EPICS CA read.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> ReadAsync(EndPointID endPointID, ReadCallback callback)
        {
            var cb = callback as ReadCallback;
            var tcs = new TaskCompletionSource<EcaType>();
            // If the callback is null, don't bother trying to read. ca_array_get_callback will return ECA_BADFUNCPTR
            // Read must have a valid callback to be able to access the read data.
            if (cb == null)
            {
                tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
            else
            {
                if (!_epics_ca_connections!.ContainsKey(endPointID))
                {
                    tcs.SetResult(EcaType.ECA_DISCONN);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }
                var epicsSettings = _epics_ca_connections[endPointID];
                if (epicsSettings.ChannelHandle == nint.Zero)
                {
                    tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }

                var getResult = ChannelAccessWrapper.ca_array_get_callback(
                pChanID: epicsSettings.ChannelHandle,
                type: epicsSettings.DataType,
                nElements: epicsSettings.ElementCount,
                valueUpdateCallBack: cb);
                // Do the pend event to block until the callback is invoked.
                if (ChannelAccessWrapper.ca_pend_event(EPICS_TIMEOUT_SEC) == EcaType.ECA_EVDISALLOW)
                    tcs.SetResult(EcaType.ECA_EVDISALLOW);
                if (getResult != EcaType.ECA_NORMAL)
                {
                    tcs.SetResult(getResult);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }

                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                var result = ChannelAccessWrapper.ca_flush_io();
                tcs.SetResult(result);

                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
        }

        /// <summary>
        /// Handles the EPICS CA subscription.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="endPointID"></param>
        /// <param name="monitorType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> SubscribeAsync<T1, T2>(EndPointID endPointID, T1 monitorType, T2 callback)
        {
            var cb = callback as MonitorCallback;
            var tcs = new TaskCompletionSource<EcaType>();
            var monType = monitorType as MonitorTypes?;
            // Monitor should always have a valid callback.
            if (cb == null)
            {
                tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
            else
            {
                if (!_epics_ca_connections!.ContainsKey(endPointID))
                {
                    tcs.SetResult(EcaType.ECA_DISCONN);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }
                else
                {
                    var epicsSettings = _epics_ca_connections[endPointID];
                    if (epicsSettings.ChannelHandle == nint.Zero)
                    {
                        tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }
                    var result = ChannelAccessWrapper.ca_create_subscription(
                                    pChanID: epicsSettings.ChannelHandle,
                                    dbrType: epicsSettings.DataType,
                                    count: epicsSettings.ElementCount,
                                    whichFieldsToMonitor: monType??MonitorTypes.MonitorValField,
                                    valueUpdateCallback: cb,
                                    out epicsSettings.MonitorHandle);
                    if (result != EcaType.ECA_NORMAL)
                    {
                        tcs.SetResult(result);
                        return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                    }
                    // Do the pend event to block until the callback is invoked.
                    result = ChannelAccessWrapper.ca_pend_event(EPICS_TIMEOUT_SEC);
                    if (result == EcaType.ECA_TIMEOUT) // ca_pend_event() returns ECA_TIMEOUT if successful.
                        tcs.SetResult(EcaType.ECA_NORMAL);
                    else
                        tcs.SetResult(result);
                }
                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
        }
   
        /// <summary>
        /// Handles the EPICS CA write.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointID"></param>
        /// <param name="pvalue"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task<EndPointStatus> WriteAsync<T>(EndPointID endPointID, nint pvalue, T? callback)
        {
            var cb = callback as WriteCallback;
            var tcs = new TaskCompletionSource<EcaType>();
            // If the pvalue, don't bother trying to write.
            if (pvalue == nint.Zero)
            {
                tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
            if (!_epics_ca_connections!.ContainsKey(endPointID))
            {
                tcs.SetResult(EcaType.ECA_DISCONN);
                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
            else
            {
                var epicsSettings = _epics_ca_connections[endPointID];
                if (epicsSettings.ChannelHandle == nint.Zero)
                {
                    tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }
                EcaType result = EcaType.ECA_NOSUPPORT;
                if (cb == null)
                    result = ChannelAccessWrapper.ca_array_put(
                                pChanID: epicsSettings.ChannelHandle,
                                dbrType: epicsSettings.DataType,
                                nElements: epicsSettings.ElementCount,
                                pValueToWrite: pvalue);
                else
                    result = ChannelAccessWrapper.ca_array_put_callback(
                                pChanID: epicsSettings.ChannelHandle,
                                dbrType: epicsSettings.DataType,
                                nElements: epicsSettings.ElementCount,
                                ptrValueToWrite: pvalue,
                                writeCallback: cb);
                if (result != EcaType.ECA_NORMAL)
                {
                    tcs.SetResult(result);
                    return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
                }
                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                var flushResult = ChannelAccessWrapper.ca_flush_io();
                tcs.SetResult(flushResult);
                return Task.FromResult(EcaTypeToEndPointStatus(tcs.Task.Result));
            }
        }

        EndPointStatus EcaTypeToEndPointStatus(EcaType eps)
        {
            EndPointStatus status = EndPointStatus.UnknownError;
            switch (eps)
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
            return status;
        }
    }
}
