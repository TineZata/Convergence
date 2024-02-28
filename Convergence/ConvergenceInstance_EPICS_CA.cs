using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.ConnectionCallback;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;
using System.Reflection;
using Convergence.IO;
using static Convergence.ReadCallbackDelegate;

namespace Convergence
{
    public partial class ConvergenceInstance
    {
        /// <summary>
        /// Handles the EPICS CA connection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPointArgs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public void EpicsCaConnect<T>(EndPointBase<T> endPointArgs)
        {
            var endPointID = endPointArgs.EndPointID;
            // Check if the CA ID already exists.
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                return;
            }
            else
            {
                var epicsSettings = endPointArgs.ConvertToEPICSSettings();
                // Always call ca_context_create() before any other Channel Access calls from the thread you want to use Channel Access from.
                EPICSWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
                switch (EPICSWrapper.ca_create_channel(endPointArgs.EndPointID.EndPointName ?? "", null, out epicsSettings.ChannelHandle))
                {
                    case EcaType.ECA_NORMAL:
                        // Try add a new ID, if not already added.
                        endPointID.UniqueId = Guid.NewGuid();
                        _epics_ca_connections!.TryAdd(endPointID, epicsSettings);
                        break;

                    case EcaType.ECA_BADSTR:
                        throw new ArgumentException("Invalid channel name");
                        break;
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

        private async Task<EndPointStatus> EpicsCaReadAsync(EndPointID endPointID, ReadCallback? callback)
        {
            EndPointStatus status = EndPointStatus.Disconnected;
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                var epicsSettings = _epics_ca_connections[endPointID];
                if (epicsSettings.ChannelHandle != IntPtr.Zero)
                { 
                    EcaType result = await Task.Run(
                        () => EPICSWrapper.ca_array_get_callback(
                        pChanID: epicsSettings.ChannelHandle,
                        type: epicsSettings.DataType,
                        nElementsWanted: epicsSettings.ElementCount,
                        valueUpdateCallBack: callback!)
                        );
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
                }
                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                EPICSWrapper.ca_flush_io();
            }
            return status;
        }

        private ValueUpdateNotificationEventArgs GetEventArgs(ReadCallback callback)
        {
            ParameterInfo[] infos = callback.Method.GetParameters();

            IntPtr argsPtr = (IntPtr)infos[0].DefaultValue; // Get actual IntPtr value

            return (ValueUpdateNotificationEventArgs)Marshal.PtrToStructure(argsPtr, typeof(ValueUpdateNotificationEventArgs));
        }

        private DbRecordRequestType GetDbFieldType(DataTypes type)
        {
            Enum.TryParse(type.ToString(), out DbRecordRequestType dbReqtype);
            return dbReqtype;
        }
    }
}
