using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{
		/// <summary>
		/// EPICS caput async method, which specifies the PV name only.
		/// Default type is string, element count is 1 and callback is null.
		/// </summary>
		/// <param name="pvName"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> ConnectAsync(String pvName)
		{
			return await ConnectAsync(pvName, typeof(string), 1, _nullConnectionCallback);
		}

		/// <summary>
		/// EPICS caput async method, which specifies the PV name and the data type.
		/// Default element count is 1 and callback is null.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> ConnectAsync(String pvName, System.Type type)
		{
			return await ConnectAsync(pvName, type, 1, _nullConnectionCallback);
		}

		/// <summary>
		/// EPICS caput async method, which specifies the PV name, the data type and the element count.
		/// Default callback is null.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <param name="elements"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> ConnectAsync(String pvName, System.Type type, int elements)
		{
			return await ConnectAsync(pvName, type, elements, _nullConnectionCallback);
		}

		/// <summary>
		/// EPICS caput async method, which specifies the PV name, the data type, the element count and the callback.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <param name="elements"></param>
		/// <param name="connectCallback"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> ConnectAsync(String pvName, System.Type type, int elements, Convergence.IO.EPICS.CA.EventCallbackDelegate.ConnectCallback? connectCallback)
		{
            var endpoint = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetEpicsCaEndPointID(pvName);
            var epicsSettings = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetEpicsCaEndPointSettings(endpoint, type, elements);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endpoint, Settings = epicsSettings };
			var connResult = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, connectCallback);
			return connResult;
		}
	}
}
