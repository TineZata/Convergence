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
		/// EPICS caget async method, which specifies the PV name only.
		/// Will return the value as a string and assume element count of 1.
		/// </summary>
		/// <param name="pvName"></param>
		/// <returns></returns>
		public static async Task<CagetAsyncResult> CagetAsync(String pvName)
		{
			return await CagetAsync(pvName, typeof(string));
		}

		/// <summary>
		/// EPICS caget async method, which specifies the PV name and the data type.
		/// Will assume element count of 1.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static async Task<CagetAsyncResult> CagetAsync(String pvName, System.Type type)
		{
			return await CagetAsync(pvName, type, 1, true);
		}

		/// <summary>
		/// EPICS caget async method, which specifies the PV name, the data type and the element count.
		/// If disconnect is true, will disconnect after reading the value.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <param name="elementCount"></param>
		/// <param name="disconnect"></param>
		/// <returns></returns>
		public static async Task<CagetAsyncResult> CagetAsync(String pvName, System.Type type, int elementCount, bool disconnect)
		{
			EndPointStatus status = EndPointStatus.UnknownError;
			object value = null;
			CagetAsyncResult cagetAsyncResult = new CagetAsyncResult();
			// Starts off with a EndPoint connection to the PV
			var endpoint = new EndPointID(Protocols.EPICS_CA, pvName);
			var epicsSettings = new Convergence.IO.EPICS.CA.Settings(
				datatype: Convergence.IO.EPICS.CA.Helpers.GetDBFieldType(type),
				elementCount: elementCount);
			var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endpoint, Settings = epicsSettings };
			var connResult = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, _nullConnectionCallback);
			if (connResult == EndPointStatus.Okay)
			{
				status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endpoint, (EventCallbackDelegate.WriteCallback)((valueOut) =>
				{
					value = Convergence.IO.EPICS.CA.Helpers.DecodeEventData(valueOut);
				}));
				cagetAsyncResult = new CagetAsyncResult { Status = status, Value = value };
			}
			else
			{
				cagetAsyncResult =  new CagetAsyncResult { Status = status, Value = value};
			}
			if (disconnect)
			{
				// Artificial delay to allow the read to complete
				Task.Delay(Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.EPICS_TIMEOUT_MSEC).Wait();
				Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endpoint);
			}
			return cagetAsyncResult;
		}

	}
}
