﻿using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.ConnectionCallback;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;
using System.Reflection;
using Convergence.IO;
using static Convergence.IO.EPICS.CaEventCallbackDelegate;
using System.Threading;
using Convergence;
using Conversion.IO.EPICS;

namespace Convergence
{
    public partial class ConvergenceInstance
    {
        private double _epics_timeout = 0.5;
        /// <summary>
        /// Handles the EPICS CA connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointArgs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<EcaType> EpicsCaConnect<T>(EndPointBase<T> endPointArgs)
        {
            var tcs = new TaskCompletionSource<EcaType>();
            var endPointID = endPointArgs.EndPointID;
            // Check if the CA ID already exists.
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                tcs.SetResult(EcaType.ECA_NORMAL);
                return tcs.Task.Result;
            }
            else
            {
                var epicsSettings = endPointArgs.ConvertToEPICSSettings();
                // Always call ca_context_create() before any other Channel Access calls from the thread you want to use Channel Access from.
                var contextResult = EPICSWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
                if (contextResult != EcaType.ECA_NORMAL)
                {
                    tcs.SetResult(contextResult);
                    return tcs.Task.Result;
                }

                var chCreateResult = EPICSWrapper.ca_create_channel(endPointArgs.EndPointID.EndPointName ?? "", null, out epicsSettings.ChannelHandle);
                if (chCreateResult != EcaType.ECA_NORMAL)
                {
                    tcs.SetResult(chCreateResult);
                    return tcs.Task.Result;
                }

                if (EPICSWrapper.ca_pend_io(_epics_timeout) == EcaType.ECA_EVDISALLOW)
                {
                    tcs.SetResult(EcaType.ECA_EVDISALLOW);
                    return tcs.Task.Result;
                }

                if (chCreateResult == EcaType.ECA_NORMAL)
                {
                    // Try add a new ID, if not already added.
                    endPointID.UniqueId = Guid.NewGuid();
                    if (!_epics_ca_connections!.TryAdd(endPointID, epicsSettings))
                        tcs.SetResult(EcaType.ECA_NEWCONN); // New or resumed network connection
                    else
                        tcs.SetResult(EcaType.ECA_NORMAL);
                    return tcs.Task.Result;
                }
                else
                {
                    tcs.SetResult(chCreateResult);
                    return tcs.Task.Result;
                }
            }
        }

        /// <summary>
        /// Disconnects the EPICS CA connection.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void EpicsCaDisconnect(EndPointID endPointID)
        {
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                switch (EPICSWrapper.ca_clear_channel(_epics_ca_connections[endPointID].ChannelHandle))
                {
                    case EcaType.ECA_NORMAL:
                        _epics_ca_connections.TryRemove(endPointID, out _);
                        break;
                    case EcaType.ECA_BADCHID:
                        throw new ArgumentException("Corrupted ChannelID");
                        break;
                }
            }
        }

        private async Task<EcaType> EpicsCaReadAsync(EndPointID endPointID, CaReadCallback? callback)
        {
            var tcs = new TaskCompletionSource<EcaType>();

            if (!_epics_ca_connections!.ContainsKey(endPointID))
            {
                tcs.SetResult(EcaType.ECA_DISCONN);
                return tcs.Task.Result;
            }
            var epicsSettings = _epics_ca_connections[endPointID];
            if (epicsSettings.ChannelHandle == IntPtr.Zero)
            {
                tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                return tcs.Task.Result;
            }

            var getResult = EPICSWrapper.ca_array_get_callback(
            pChanID: epicsSettings.ChannelHandle,
            type: epicsSettings.DataType,
            nElements: epicsSettings.ElementCount,
            valueUpdateCallBack: callback!);
            // Do the pend event to block until the callback is invoked.
            if (EPICSWrapper.ca_pend_event(_epics_timeout) == EcaType.ECA_EVDISALLOW)
                tcs.SetResult(EcaType.ECA_EVDISALLOW);
            if (getResult != EcaType.ECA_NORMAL)
            {
                tcs.SetResult(getResult);
                return tcs.Task.Result;
            }

            // Must call 'flush' otherwise the message isn't sent to the server
            // immediately. If we forget to call 'flush', the message *will* eventually
            // get sent, but not until the default timeout period of 30 secs has elapsed,
            // in which case the callback handler won't be invoked until that 30 secs has elapsed.
            var result = EPICSWrapper.ca_flush_io();
            tcs.SetResult(result);

            return tcs.Task.Result;
        }

        private Task<EcaType> EpicsCaWriteAsync(EndPointID endPointID, IntPtr pvalue, CaWriteCallback? callback)
        {
            var tcs = new TaskCompletionSource<EcaType>();
            if (pvalue == null || callback == null)
            {
                tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                return tcs.Task;
            }
            if (!_epics_ca_connections!.ContainsKey(endPointID))
            {
                tcs.SetResult(EcaType.ECA_DISCONN);
                return tcs.Task;
            }
            else
            {
                var epicsSettings = _epics_ca_connections[endPointID];
                if (epicsSettings.ChannelHandle == IntPtr.Zero)
                {
                    tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                    return tcs.Task;
                }
                var result = EPICSWrapper.ca_array_put_callback(
                                pChanID: epicsSettings.ChannelHandle,
                                dbrType: epicsSettings.DataType,
                                nElements: epicsSettings.ElementCount,
                                ptrValueToWrite: pvalue,
                                writeCallback: callback);
                if (result != EcaType.ECA_NORMAL)
                {
                    tcs.SetResult(result);
                    return tcs.Task;
                }
                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                var flushResult = EPICSWrapper.ca_flush_io();
                tcs.SetResult(flushResult);
                return tcs.Task;
            }
        }

        EndPointStatus GetEPICSEndPointStatus(EcaType eps)
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

        public Task<EcaType> EpicsCaMonitor(EndPointID endPointID, CaMonitorTypes? monType, CaMonitorCallback? callback)
        {
            var tcs = new TaskCompletionSource<EcaType>();
            if (monType == null || callback == null)
            {
                tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                return tcs.Task;
            }
            if (!_epics_ca_connections!.ContainsKey(endPointID))
            {
                tcs.SetResult(EcaType.ECA_DISCONN);
                return tcs.Task;
            }
            else
            {
                var epicsSettings = _epics_ca_connections[endPointID];
                if (epicsSettings.ChannelHandle == IntPtr.Zero)
                {
                    tcs.SetResult(EcaType.ECA_BADFUNCPTR);
                    return tcs.Task;
                }
                var result = EPICSWrapper.ca_create_subscription(
                                pChanID: epicsSettings.ChannelHandle,
                                dbrType: epicsSettings.DataType,
                                count: epicsSettings.ElementCount,
                                whichFieldsToMonitor: monType,
                                valueUpdateCallback: callback);
            }
            return tcs.Task;
        }
    }
}

