using Convergence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// The channel handle.
        /// </summary>
        public IntPtr ChannelHandle = IntPtr.Zero;
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
        public DbFieldType DataType { get; set; }

        /// <summary>
        /// Number of elements.
        /// Greater than 1 if array.
        /// </summary>
        public int ElementCount { get; set; } = 1;

        /// <summary>
        /// Description of the PV.
        /// </summary>
        string ? Description { get; set; }


        public Settings(DbFieldType datatype, bool isServer, int elementCount, bool isPVA)
        {
            DataType = datatype;
            IsServer = isServer;
            ElementCount = elementCount;
            IsPVA = isPVA;
        }

        // Decode EPICS data according to the data type.
        public object DecodeData(ReadCallbackArgs args)
        {
            object data = null;
            Enum.TryParse<DbFieldType>(args.type.ToString(), out var type);
            switch(type)
            {
                case DbFieldType.DBF_SHORT_i16:
                    Int16[] shortArray = new Int16[args.count];
                    Marshal.Copy(args.dbr, shortArray, 0, args.count);
                    if (shortArray.Length == 1)
                    {
                        data = shortArray[0];
                    }
                    else
                    {
                        data = shortArray;
                    }
                    return data;
            }
            return data;
        }
    }
}
