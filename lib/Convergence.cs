using Convergence;
using Convergence.Interfaces;
using Convergence.IO.EPICS;

namespace TestIOCL
{
    public class Convergence : IConvergence
    {
        // Singleton instance of Convergence.
        private static Convergence _hub;

        /// <summary>
        /// Network communication hub, which keeps track of all the connections on all protocols.
        /// 
        /// Must be a singleton because network communication is an expensive resource and we don't want to have multiple instances of it.
        /// </summary>
        public static Convergence Hub
        {
            get
            {
                if (_hub == null)
                {
                    _hub = new Convergence();
                }
                return _hub;
            }
        }
        // Private constructor for singleton, to prevent external instantiation.
        private Convergence() { }
        
        public EndPointID Connect<T>(EndPointID id, EndPointBase<T> endPointArgs)
        {
            return EndPointID.Empty;
        }
    }
}