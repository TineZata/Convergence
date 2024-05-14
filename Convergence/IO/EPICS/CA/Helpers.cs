using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
    public static class Helpers
    {
        // Decode EPICS data according to the data type.
        public static object DecodeEventData(EventCallbackArgs args)
        {
            object data = null;
            Enum.TryParse<DbFieldType>(args.type.ToString(), out var type);
            switch (type)
            {
                // Decode data for and BDF_SHORT_i16
                case DbFieldType.DBF_SHORT_i16:
                case DbFieldType.DBF_ENUM_i16:
                    short[] shortArray = new short[args.count];
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
                    int[] longArray = new int[args.count];
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
                    string stringArray = Marshal.PtrToStringAnsi(args.dbr);
                    data = stringArray;
                    return data;
            }
            return data;
        }

        public static DbFieldType GetDBFieldType(Type sType)
        {
            switch (sType)
            {
                case Type type when type == typeof(bool):
                case Type t when t == typeof(Enum):
                    return DbFieldType.DBF_ENUM_i16;
                case Type t when t == typeof(byte):
                    return DbFieldType.DBF_CHAR_byte;
                case Type t when t == typeof(short):
                    return DbFieldType.DBF_SHORT_i16;
                case Type t when t == typeof(int):
                    return DbFieldType.DBF_LONG_i32;
                case Type t when t == typeof(float):
                    return DbFieldType.DBF_FLOAT_f32;
                case Type t when t == typeof(double):
                    return DbFieldType.DBF_DOUBLE_f64;
                case Type t when t == typeof(string):
                default:
                    return DbFieldType.DBF_STRING_s39;
            }
        }
        /// <summary>
        /// Get the EndPointID from the PV name.
        /// </summary>
        /// <param name="pvName"></param>
        /// <returns></returns>
		internal static EndPointID GetEndPointID(String pvName)
		{
            foreach (var kvp in ConvergeOnEPICSChannelAccess.Hub.ConnectionsInstance)
            {
				EndPointID endpointId = kvp.Key;
				if (endpointId.EndPointName == pvName)
                {
					return endpointId;
				}
            }
            return new EndPointID(Protocols.None, "");
		}
	}
}
