using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.EPICS.ConnectionCallback;
using Convergence.IO.EPICS;

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
        public EndPointID EpicsCaConnect<T>(EndPointBase<T> endPointArgs)
        {
            var endPointID = endPointArgs.EndPointID;
            // Check if the CA ID already exists.
            if (_epics_ca_connections!.ContainsKey(endPointID))
            {
                return endPointID;
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
                        return endPointID;

                    case EcaType.ECA_BADSTR:
                        throw new ArgumentException("Invalid channel name");
                        break;
                }
                return new EndPointID(Protocols.None, Guid.Empty);
            }
        }

        /// <summary>
        /// Disconnects the EPICS CA connection.
        /// </summary>
        /// <param name="endPointID"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void EpicsCaDisconnect(EndPointID endPointID)
        {
            switch(endPointID.Protocol)
            {
                case Protocols.EPICS_CA:
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
                    break;
                default:
                    throw new NotImplementedException();
            }
            
        }
    }
}
