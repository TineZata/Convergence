using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS
{
    /// <summary>
    /// Settings for EPICS protocol.
    /// 
    /// EPICS only requires the distintion between CA and PVA.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// True if the protocol is PVA.
        /// </summary>
        public bool IsPVA { get; set; }

        /// <summary>
        /// True if the protocol is server.
        /// A server would contain db records, while a client would not.
        /// </summary>
        public bool IsServer { get; set; }

        public Settings(bool isPVA, bool isServer)
        {
            IsPVA = isPVA;
            IsServer = isServer;

        }
    }
}
