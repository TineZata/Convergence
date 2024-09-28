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
        public static readonly double EPICS_TIMEOUT_SEC = 0.15;

        /// <summary>
        /// Singleton instance of Convergence on EPICS Channel Access.
        /// </summary>
        private static ConvergeOnEPICSChannelAccess? _hub;
        /// <summary>
        /// ConcurrentDictionary of all the EPICS CA connections as a key-value pair of EndPointID and EndPointBase<EPICSSettings>.
        /// </summary>
        private static System.Collections.Concurrent.ConcurrentDictionary<EndPointID, Settings>? _epics_ca_connections = new();
		/// <summary>
		/// Lock object
		/// </summary>
		private static readonly object _lock = new object();

		/// <summary>
		/// Snapshot of all the EPICS CA connections.
		/// </summary>
		public System.Collections.Concurrent.ConcurrentDictionary<EndPointID, Settings> ConnectionsInstance
        {
			get => _epics_ca_connections!;
		}

        // Get EndPointID from the dictionary using the PV name.
        public EndPointID GetEpicsCaEndPointID(string pvName)
        {
            var epicsSettings = ConnectionsInstance.FirstOrDefault(x => x.Key.EndPointName == pvName);
            if (epicsSettings.Key == null)
                return new EndPointID(Protocols.EPICS_CA, pvName);
            else
            return epicsSettings.Key;
        }

        public Settings GetEpicsCaEndPointSettings(EndPointID endPointID, System.Type type, int elements)
        {
            if (ConnectionsInstance.ContainsKey(endPointID))
                return ConnectionsInstance[endPointID];
            else
                return new Settings(Helpers.GetDBFieldType(type), elements);
        }

        /// <summary>
        /// Network communication hub, which keeps track of all the connections on all EPICS Channels Access endpoints.
        /// </summary>
        public static ConvergeOnEPICSChannelAccess Hub
        {
			get
			{
                if (_hub == null)
                {
                    lock (_lock)
                    {
                        _hub = new ConvergeOnEPICSChannelAccess();
                    }
                }
				return _hub;
			}
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
                //var tcs = new TaskCompletionSource<EcaType>();
                // Check if the CA ID already exists.
                if (ConnectionsInstance.ContainsKey(endPointArgs.EndPointID))
                {
                    settings = ConnectionsInstance[endPointArgs.EndPointID];
					// Log message that an existing channel is being used... with its unique ID.
					Console.WriteLine($"Using existing channel for {endPointArgs.EndPointID.EndPointName} with ID {endPointArgs.EndPointID.UniqueId}");
					return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_NORMAL));
                }
                else
                {
                    // Log message that a new channel is being created
                    Console.WriteLine($"Creating a new channel for {endPointArgs.EndPointID.EndPointName}");
					// Always call ca_context_create() before any other Channel Access calls from the thread you want to use Channel Access from.
					var contextResult = ChannelAccessWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
                    if (contextResult != EcaType.ECA_NORMAL)
                    {
                        if (contextResult == EcaType.ECA_NOTTHREADED)
                        {
							// Log message that ECA_NOTTHREADED was returned and the current thread is not a member of a non-preemptive callback CA context.
							Console.WriteLine($"Error creating context for {endPointArgs.EndPointID.EndPointName} - {contextResult}, non-preemptive thread was created, but will be destroyed.");
							// Current thread is already a member of a non-preemptive callback CA context (possibly created implicitly)
							// Forcibly destroy the current context and attempt annother context_create
							ChannelAccessWrapper.ca_context_destroy();
							// Try another connect create
							contextResult = ChannelAccessWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
                            if (contextResult != EcaType.ECA_NORMAL)
                            {
								Console.WriteLine($"Error creating 2nd context for {endPointArgs.EndPointID.EndPointName} - {contextResult}");
								return Task.FromResult(EcaTypeToEndPointStatus(contextResult));
							}
						}
						else
                        {

                            Console.WriteLine($"Error creating context for {endPointArgs.EndPointID.EndPointName} - {contextResult}");
                            return Task.FromResult(EcaTypeToEndPointStatus(contextResult));
                        }
                    }

                    // Do a ca_pend_event to make sure any residual events are cleared.
                    ChannelAccessWrapper.ca_pend_event(EPICS_TIMEOUT_SEC);

					// Do connection with a connection callback.
					EcaType chCreateResult = EcaType.ECA_DISCONN;
                    chCreateResult = ChannelAccessWrapper.ca_create_channel(
                                           endPointArgs.EndPointID.EndPointName ?? "",
                                            callback,
                                            out settings.ChannelHandle);
                    if (chCreateResult != EcaType.ECA_NORMAL)
                    {
                        Console.WriteLine($"Error creating channel for {endPointArgs.EndPointID.EndPointName} - {chCreateResult}");
						return Task.FromResult(EcaTypeToEndPointStatus(chCreateResult));
                    }

                    // If the callback is not null the channel access does not block on a pend_io, 
                    // however a call is still required to flush the IO.
                    if (ChannelAccessWrapper.ca_pend_io(EPICS_TIMEOUT_SEC) == EcaType.ECA_EVDISALLOW)
                    {
                        Console.WriteLine($"Error creating channel for {endPointArgs.EndPointID.EndPointName} - {EcaType.ECA_EVDISALLOW}");
						return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_EVDISALLOW));
                    }

                    if (chCreateResult == EcaType.ECA_NORMAL)
                    {
                        // Try add a new ID, if not already added.
                        endPointArgs.EndPointID.UniqueId = Guid.NewGuid();
                        if (!ConnectionsInstance.TryAdd(endPointArgs.EndPointID, settings))
                        {
                            Console.WriteLine($"Error adding channel for {endPointArgs.EndPointID.EndPointName} - {EcaType.ECA_NOSUPPORT}");
                            return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_NEWCONN)); // New or resumed network connection
                        }
                        else
                        {

                            Console.WriteLine($"Channel created for {endPointArgs.EndPointID.EndPointName} - {EcaType.ECA_NORMAL}");
							return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_NORMAL));
                        }
                    }
                    else
                    {

                        Console.WriteLine($"Error creating channel for {endPointArgs.EndPointID.EndPointName} - {chCreateResult}");
						return Task.FromResult(EcaTypeToEndPointStatus(chCreateResult));
                    }
                }
            }
        }

        /// <summary>
        /// Handles the EPICS CA disconnection.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <returns></returns>
        public Task <bool> DisconnectAsync(EndPointID endPointID)
        {
            bool disconnected = false;
            if (ConnectionsInstance.ContainsKey(endPointID))
            {
				if (ConnectionsInstance[endPointID].SubscriptionHandle != nint.Zero)
					ChannelAccessWrapper.ca_clear_subscription(ConnectionsInstance[endPointID].SubscriptionHandle);
				ChannelAccessWrapper.ca_clear_channel(ConnectionsInstance[endPointID].ChannelHandle);
				ChannelAccessWrapper.ca_pend_io(EPICS_TIMEOUT_SEC);
				disconnected = ConnectionsInstance.Remove(endPointID, out _);
				// Log endpointId has been removed from the dictionary.
				Console.WriteLine($"EndPointID {endPointID.EndPointName} - {endPointID.UniqueId} has been removed from the dictionary.");
			}
			else
			{
				// Log endpointId does not exist in the dictionary.
				Console.WriteLine($"EndPointID {endPointID.EndPointName} does not exist in the dictionary... Assume it is already disconnected.");
				// If it doesn't exist, it is already disconnected.
				disconnected = true;
			}
			return Task.FromResult(disconnected);
        }

		/// <summary>
		/// Handles the EPICS CA read.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="endPointID"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public Task<EndPointStatus> ReadAsync<T>(EndPointID endPointID, T callback)
		{
            var cb = callback as ReadCallback;
            // If the callback is null, don't bother trying to read. ca_array_get_callback will return ECA_BADFUNCPTR
            // Read must have a valid callback to be able to access the read data.
            if (cb == null)
            {
				return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_BADFUNCPTR));
            }
            else
            {
                if (!ConnectionsInstance.ContainsKey(endPointID))
                {
					return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_DISCONN));
                }
                var epicsSettings = ConnectionsInstance[endPointID];
                if (epicsSettings.ChannelHandle == nint.Zero)
                {
					return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_BADFUNCPTR));
                }

                var getResult = ChannelAccessWrapper.ca_array_get_callback(
                pChanID: epicsSettings.ChannelHandle,
                type: epicsSettings.DataType,
                nElements: epicsSettings.ElementCount,
                valueUpdateCallBack: cb);
                // Do the pend event to block until the callback is invoked.
                if (ChannelAccessWrapper.ca_pend_event(EPICS_TIMEOUT_SEC) == EcaType.ECA_EVDISALLOW)
					return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_EVDISALLOW));
                if (getResult != EcaType.ECA_NORMAL)
                {
					return Task.FromResult(EcaTypeToEndPointStatus(getResult));
                }

                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                var result = ChannelAccessWrapper.ca_flush_io();
				return Task.FromResult(EcaTypeToEndPointStatus(result));
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
            var monType = monitorType as MonitorTypes?;
            // Monitor should always have a valid callback.
            if (cb == null)
            {
				return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_BADFUNCPTR));
            }
            else
            {
                if (!ConnectionsInstance.ContainsKey(endPointID))
                {
					return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_DISCONN));
                }
                else
                {
                    var epicsSettings = ConnectionsInstance[endPointID];
                    if (epicsSettings.ChannelHandle == nint.Zero)
                    {
						return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_BADFUNCPTR));
                    }
                    var result = ChannelAccessWrapper.ca_create_subscription(
                                    pChanID: epicsSettings.ChannelHandle,
                                    dbrType: epicsSettings.DataType,
                                    count: epicsSettings.ElementCount,
                                    whichFieldsToMonitor: monType??MonitorTypes.MonitorValField,
                                    valueUpdateCallback: cb,
                                    out epicsSettings.SubscriptionHandle);
                    if (result != EcaType.ECA_NORMAL)
                    {
						return Task.FromResult(EcaTypeToEndPointStatus(result));
                    }
                    // Do the pend event to block until the callback is invoked.
                    result = ChannelAccessWrapper.ca_pend_event(EPICS_TIMEOUT_SEC);
                    if (result == EcaType.ECA_TIMEOUT) // ca_pend_event() returns ECA_TIMEOUT if successful.
                        return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_NORMAL));
                    else
						return Task.FromResult(EcaTypeToEndPointStatus(result));
                }
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
            // If the pvalue, don't bother trying to write.
            if (pvalue == nint.Zero)
            {
				return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_BADFUNCPTR));
            }
            if (!ConnectionsInstance.ContainsKey(endPointID))
            {
				return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_DISCONN));
            }
            else
            {
                var epicsSettings = ConnectionsInstance[endPointID];
                if (epicsSettings.ChannelHandle == nint.Zero)
                {
					return Task.FromResult(EcaTypeToEndPointStatus(EcaType.ECA_BADFUNCPTR));
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
					return Task.FromResult(EcaTypeToEndPointStatus(result));
                }
                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                var flushResult = ChannelAccessWrapper.ca_flush_io();
				return Task.FromResult(EcaTypeToEndPointStatus(flushResult));
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
