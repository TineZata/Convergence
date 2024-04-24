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

        /// <summary>
        /// Pointer to the monitor handle.
        /// </summary>
        public IntPtr MonitorHandle  = IntPtr.Zero;

        /// <summary>
        /// Pointer to the write handle.
        /// 
        public IntPtr WriteHandle = IntPtr.Zero;


        public Settings(DbFieldType datatype, bool isServer, int elementCount, bool isPVA)
        {
            DataType = datatype;
            IsServer = isServer;
            ElementCount = elementCount;
            IsPVA = isPVA;
        }

        // Decode EPICS data according to the data type.
        public object DecodeEventData(EventCallbackArgs args)
        {
            object data = null;
            Enum.TryParse<DbFieldType>(args.type.ToString(), out var type);
            switch(type)
            {
                // Decode data for and BDF_SHORT_i16
                case DbFieldType.DBF_SHORT_i16:
                case DbFieldType.DBF_ENUM_i16:
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
                // Decode data for and DBF_LONG_i32
                case DbFieldType.DBF_LONG_i32:
                    Int32[] longArray = new Int32[args.count];
                    Marshal.Copy(args.dbr, longArray, 0, args.count);
                    if (longArray.Length == 1)
                    {
                        data = longArray[0];
                    }
                    else
                    {
                        data = longArray;
                    }
                    return data;
                // Decode data for and DBF_FLOAT_f32
                case DbFieldType.DBF_FLOAT_f32:
                    float[] floatArray = new float[args.count];
                    Marshal.Copy(args.dbr, floatArray, 0, args.count);
                    if (floatArray.Length == 1)
                    {
                        data = floatArray[0];
                    }
                    else
                    {
                        data = floatArray;
                    }
                    return data;
                // Decode data for and DBF_DOUBLE_f64 which take data from an two array items of 32 bits.
                case DbFieldType.DBF_DOUBLE_f64:
                    double[] doubleArray = new double[args.count];
                    Marshal.Copy(args.dbr, doubleArray, 0, args.count);
                    if (doubleArray.Length == 1)
                    {
                        data = doubleArray[0];
                    }
                    else
                    {
                        data = doubleArray;
                    }
                    return data;

                // Decode data for and DBF_STRING
                case DbFieldType.DBF_STRING_s39:
                    string[] stringArray = new string[args.count];
                    for (int i = 0; i < args.count; i++)
                    {
                        stringArray[i] = Marshal.PtrToStringAnsi(args.dbr);
                    }
                    if (stringArray.Length == 1)
                    {
                        data = stringArray[0];
                    }
                    else
                    {
                        data = stringArray;
                    }
                    return data;
            }
            return data;
        }
    }
}
