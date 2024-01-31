using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.ConnectionCallback;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;
using System.Reflection;
using Convergence.IO;

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

        private async Task EpicsCaReadAsync(EndPointID endPointID, ValueUpdateCallback? callback)
        {
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                var epicsSettings = _epics_ca_connections[endPointID];
                if (epicsSettings.ChannelHandle != IntPtr.Zero)
                { 
                    EcaType result = await Task.Run(
                        () => EPICSWrapper.ca_array_get_callback(
                        pChanID: epicsSettings.ChannelHandle,
                        type: GetDbFieldType(epicsSettings.DataType),
                        nElementsWanted: epicsSettings.ElementCount,
                        valueUpdateCallBack: callback!,
                        userArg: GetEventArgs(callback!).tagValue));
                    switch (result)
                    {
                        case EcaType.ECA_NORMAL:
                            break;
                        case EcaType.ECA_BADTYPE:
                            throw new ArgumentException("Invalid type");
                            break;
                        case EcaType.ECA_BADCOUNT:
                            throw new ArgumentException("Invalid count");
                            break;
                        case EcaType.ECA_NORDACCESS:
                            throw new ArgumentException("No read access");
                            break;
                        case EcaType.ECA_DISCONN:
                            throw new ArgumentException("Channel disconnected");
                            break;
                        case EcaType.ECA_UNAVAILINSERV:
                            throw new ArgumentException("Unsupported by service");
                            break;
                        case EcaType.ECA_TIMEOUT:
                            throw new ArgumentException("Request timed out");
                            break;
                        case EcaType.ECA_ALLOCMEM:
                            throw new ArgumentException("Memory allocation failed");
                            break;
                        case EcaType.ECA_TOLARGE:
                            throw new ArgumentException("Message body too large");
                            break;
                        case EcaType.ECA_GETFAIL:
                        default:
                            throw new ArgumentException("Get failed");
                            break;
                    }
                }
                // Must call 'flush' otherwise the message isn't sent to the server
                // immediately. If we forget to call 'flush', the message *will* eventually
                // get sent, but not until the default timeout period of 30 secs has elapsed,
                // in which case the callback handler won't be invoked until that 30 secs has elapsed.
                EPICSWrapper.ca_flush_io();
            }
        }

        private ValueUpdateNotificationEventArgs GetEventArgs(ValueUpdateCallback callback)
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
