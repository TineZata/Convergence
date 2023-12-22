using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvergenceLib.IO.Tango
{
    /// <summary>
    /// Settings for Tango protocol.
    /// 
    /// Tango requires destintion between CORBA and ZMQ.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// True if the protocol is ZMQ.
        /// </summary>
        public bool IsZMQ { get; set; }
        public Settings(bool isZMQ)
        {
            IsZMQ = isZMQ;
        }
    }
}
