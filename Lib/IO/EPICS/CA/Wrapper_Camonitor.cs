using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{

		/// <summary>
		/// EPICS camonitor async method, which specifies the PV name, the data type and the element count.
		/// If disconnect is true, will disconnect after reading the value.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <param name="elementCount"></param>
		/// <param name="disconnect"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> CamonitorAsync(String pvName, System.Type type,
			Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback monitorCallback)
		{
			EndPointStatus status = EndPointStatus.UnknownError;
            // Starts off with a EndPoint connection to the PV
            var endpoint = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetOrCreateEndPoint(pvName);
            var epicsSettings = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetOrCreateEndPointSettings(endpoint, type, 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endpoint, Settings = epicsSettings };
			//var connResult = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, _nullConnectionCallback);
			//if (connResult == EndPointStatus.Okay)
			//{
			status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.SubscribeAsync<MonitorTypes, Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback>
				(endpoint, MonitorTypes.MonitorValField, monitorCallback);
			//}
			//else
			//{
			//	status = connResult;
			//}
			return status;
		}
	}
}
