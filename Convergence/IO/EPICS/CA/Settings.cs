using Convergence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
    /// <summary>
    /// Settings for the EPICS Channel Access protocol.
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// The channel handle.
        /// </summary>
        public nint ChannelHandle = nint.Zero;
        /// <summary>
        /// The data type.
        /// </summary>
        public DbFieldType DataType { get; set; }

        /// <summary>
        /// Number of elements.
        /// Greater than 1 if array.
        /// </summary>
        public int ElementCount { get; set; } = 1;

        /// <summary>
        /// Description of the PV.
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// Pointer to the monitor handle.
        /// </summary>
        public nint MonitorHandle = nint.Zero;

        /// <summary>
        /// Pointer to the write handle.
        /// 
        public nint WriteHandle = nint.Zero;


        public Settings(DbFieldType datatype, int elementCount)
        {
            DataType = datatype;
            ElementCount = elementCount;
        }
    }
}
