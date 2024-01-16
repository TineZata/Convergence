using Convergence.Interfaces;
using EPICSWrapper = Convergence.IO.EPICS.ChannelAccessDLLWrapper;
using EPICSCallBack = Convergence.IO.EPICS.ConnectionCallback;
using Convergence.IO.EPICS;

namespace Convergence
{
    public class ConvergenceInstance : IConvergence
    {
        // Singleton instance of Convergence.
        private static ConvergenceInstance _hub;

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
        
        public EndPointID Connect<T>(EndPointID id, EndPointBase<T> endPointArgs)
        {
            switch (id.Protocol)
            {
                case Protocols.EPICS:
                    // Create a ca_context_create() and ca_create_channel() for EPICS Channel Access.
                    switch (EPICSWrapper.ca_context_create(PreemptiveCallbacks.DISABLE))
                    {
                        case EcaType.ECA_ALLOCMEM:
                            break;
                        case EcaType.ECA_NOTTHREADED:
                            break;
                        case EcaType.ECA_NORMAL:
                        default:
                            // Create a new channel for EPICS Channel Access.
                            IntPtr epicsHandle;
                            EPICSWrapper.ca_create_channel(id.EndPointName, null, out epicsHandle);
                            break;
                    }
                            

                    return EndPointID.Empty;
            }
            return EndPointID.Empty;
        }
    }
}