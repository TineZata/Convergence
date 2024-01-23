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
            var epicsSettings = endPointArgs.ConvertToEPICSSettings();
            // Always call ca_context_create() before any other Channel Access calls from the thread you want to use Channel Access from.
            EPICSWrapper.ca_context_create(PreemptiveCallbacks.ENABLE);
            switch (EPICSWrapper.ca_create_channel(endPointArgs.Id.EndPointName ?? "", null, out epicsSettings.ChannelHandle))
            {
                case EcaType.ECA_NORMAL:
                    
                    // Check if the CA ID already exists.
                    if (_epics_ca_connections!.ContainsKey(endPointArgs.Id))
                    {
                        return endPointArgs.Id;
                    }
                    else
                    {
                        // Try add a new ID, if not already added.
                        endPointArgs.Id.UniqueId = Guid.NewGuid();
                        _epics_ca_connections!.TryAdd(endPointArgs.Id, epicsSettings);
                        return endPointArgs.Id;
                    }
                    break;
                case EcaType.ECA_BADSTR:
                    throw new ArgumentException("Invalid channel name");
                    break;
            }
            return new EndPointID(Protocols.None, Guid.Empty);
        }
    }
}
