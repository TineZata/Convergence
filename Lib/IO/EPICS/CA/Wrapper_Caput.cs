using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CaputCallback = Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{
		/// <summary>
		/// EPICS caput async method, which specifies the PV name and value.
		/// Default type is string.
		/// Defualt element count is 1.
		/// PV will be disconnected after writing the value.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> CaputAsync(String pvName, object value)
		{
			return await CaputAsync(pvName, value, typeof(string));
		}
		/// <summary>
		/// EPICS caput async method, which specifies the PV name and the data type.
		/// Default element count is 1.
		/// PV will be disconnected after writing the value.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> CaputAsync(String pvName, object value, System.Type type)
		{
			return await CaputAsync(pvName, value, type, 1, null);
		}
		/// <summary>
		/// EPICS caput async method, which specifies the PV name, the data type and the element count.
		/// If disconnect is true, will disconnect after writing the value.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="elementCount"></param>
		/// <param name="disconnect"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> CaputAsync(String pvName, object value, System.Type type, int elementCount, CaputCallback? callback)
		{
			EndPointStatus status = EndPointStatus.UnknownError;
			// Starts off with a EndPoint connection to the PV
			var endpoint = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetEpicsCaEndPointID(pvName);

			GCHandle handle;
			if (type == typeof(string))
			{
				status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<CaputCallback>(endpoint,
					Marshal.StringToHGlobalAnsi((string)value), callback);
			}
			else {
				if (type == typeof(double))
					handle = GCHandle.Alloc((double)value, GCHandleType.Pinned);
				else if (type == typeof(float))
					handle = GCHandle.Alloc((float)value, GCHandleType.Pinned);
				else if (type == typeof(int))
					handle = GCHandle.Alloc((int)value, GCHandleType.Pinned);
				else if (type == typeof(short))
					handle = GCHandle.Alloc((short)value, GCHandleType.Pinned);
				else
					handle = GCHandle.Alloc(value, GCHandleType.Pinned);

				nint valuePtr = handle.AddrOfPinnedObject();
				status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<CaputCallback>(endpoint, valuePtr, callback);
			}
			
			return status;
		}

	}
}
