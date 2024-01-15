using Convergence.Interfaces;
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
    public class Settings : ISettings
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

        /// <summary>
        /// The data type.
        /// </summary>
        public DataTypes DataType { get; set; }

        /// <summary>
        /// Number of elements.
        /// Greater than 1 if array.
        /// </summary>
        public int ElementCount { get; set; } = 1;

        /// <summary>
        /// Description of the PV.
        /// </summary>
        string ? Description { get; set; }


        public Settings(DataTypes datatype, bool isServer, int elementCount, bool isPVA)
        {
            DataType = datatype;
            IsServer = isServer;
            ElementCount = elementCount;
            IsPVA = isPVA;
        }
    }
}
