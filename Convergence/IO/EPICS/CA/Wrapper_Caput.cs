using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
		public static async Task<EndPointStatus> CaPutAsync(String pvName, object value)
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
			return await CaPutAsync(pvName, value, type, 1, true);
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
		public static async Task<EndPointStatus> CaPutAsync(String pvName, object value, System.Type type, int elementCount, bool disconnect)
		{
			EndPointStatus status = EndPointStatus.UnknownError;
			// Starts off with a EndPoint connection to the PV
			var endpoint = new EndPointID(Protocols.EPICS_CA, pvName);
			var epicsSettings = new Convergence.IO.EPICS.CA.Settings(
				datatype: Convergence.IO.EPICS.CA.Helpers.GetDBFieldType(type),
				elementCount: elementCount);
			var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endpoint, Settings = epicsSettings };
			var connResult = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, _nullConnectionCallback);
			if (connResult == EndPointStatus.Okay)
			{
				GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);
				nint valuePtr = handle.AddrOfPinnedObject();
				status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync(endpoint, valuePtr, _nullWriteCallback);
			}
			if (disconnect)
			{
				// Artificial delay to allow the write to complete
				Task.Delay(Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.EPICS_TIMEOUT_MSEC).Wait();
				Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endpoint);
			}
			return status;
		}

	}
}
