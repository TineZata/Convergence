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


        public static async Task<string[]> GetFullMenuListAsync(string pvName)
        {
            List<string> menu  = new List<string>();
			// Get the zero string
			var zero = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".ZRST");
			menu.Add(zero.Value != null ? (string)zero.Value : string.Empty);
			// Get the one string
			var one = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".ONST");
			menu.Add(one.Value != null ? (string)one.Value : string.Empty);
			// Get the two string
			var two = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".TWST");
			menu.Add(two.Value != null ? (string)two.Value : string.Empty);
			// Get the three string
			var three = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".THST");
			menu.Add(three.Value != null ? (string)three.Value : string.Empty);
			// Get the four string
			var four = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".FRST");
			menu.Add(four.Value != null ? (string)four.Value : string.Empty);
			// Get the five string
			var five = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".FVST");
			menu.Add(five.Value != null ? (string)five.Value : string.Empty);
			// Get the six string
			var six = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".SXST");
			menu.Add(six.Value != null ? (string)six.Value : string.Empty);
			// Get the seven string
			var seven = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".SVST");
			menu.Add(seven.Value != null ? (string)seven.Value : string.Empty);
			// Get the eight string
			var eight = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".EIST");
			menu.Add(eight.Value != null ? (string)eight.Value : string.Empty);
			// Get the nine string
			var nine = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".NIST");
			menu.Add(nine.Value != null ? (string)nine.Value : string.Empty);
			// Get the ten string
			var ten = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".TEST");
			menu.Add(ten.Value != null ? (string)ten.Value : string.Empty);
			// Get the eleven string
			var eleven = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".ELST");
			menu.Add(eleven.Value != null ? (string)eleven.Value : string.Empty);
			// Get the twelve string
			var twelve = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".TVST");
			menu.Add(twelve.Value != null ? (string)twelve.Value : string.Empty);
			// Get the thirteen string
			var thirteen = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".TTST");
			menu.Add(thirteen.Value != null ? (string)thirteen.Value : string.Empty);
			// Get the fourteen string
			var fourteen = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".FTST");
			menu.Add(fourteen.Value != null ? (string)fourteen.Value : string.Empty);
			// Get the fifteen string
			var fifteen = await Convergence.IO.EPICS.CA.Wrapper.CagetAsync(pvName + ".FFST");
			menu.Add(fifteen.Value != null ? (string)fifteen.Value : string.Empty);

            return menu.ToArray();
		}
	}
}
