using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                case Type t when t == typeof(long):
					return DbFieldType.DBF_LONG_i32;
				case Type t when t == typeof(string):
                default:
                    return DbFieldType.DBF_STRING_s39;
            }
        }

        public static DbFieldType GetDBCtrlType(Type sType)
		{
			switch (sType)
			{
				case Type type when type == typeof(bool):
				case Type t when t == typeof(Enum):
					return DbFieldType.DBR_CTRL_ENUM;
				case Type t when t == typeof(byte):
					return DbFieldType.DBR_CTRL_CHAR;
				case Type t when t == typeof(short):
					return DbFieldType.DBR_CTRL_SHORT;
				case Type type when type == typeof(int):
				case Type t when t == typeof(long):
					return DbFieldType.DBR_CTRL_LONG;
				case Type t when t == typeof(float):
					return DbFieldType.DBR_CTRL_FLOAT;
				case Type t when t == typeof(double):
					return DbFieldType.DBR_CTRL_DOUBLE;
				case Type t when t == typeof(string):
				default:
					return DbFieldType.DBR_CTRL_INT;
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

		public static object GetControlStructFromDbFieldType(DbFieldType type)
		{
			switch (type)
			{
				case DbFieldType.DBR_CTRL_ENUM:
					return (object)new DBR_CTRL_ENUM();
				case DbFieldType.DBR_CTRL_SHORT:
					return (object)new DBR_CTRL_SHORT();
				case DbFieldType.DBR_CTRL_LONG:
					return (object)new DBR_CTRL_LONG();
				case DbFieldType.DBR_CTRL_FLOAT:
					return (object)new DBR_CTRL_FLOAT();
				case DbFieldType.DBR_CTRL_DOUBLE:
					return (object)new DBR_CTRL_DOUBLE();
				default:
					return (object)new DBR_CTRL_INT();
			}
		}

	}
}
