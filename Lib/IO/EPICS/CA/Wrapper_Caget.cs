using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{
		/// <summary>
		/// EPICS caget async method, which specifies the PV name, the data type and the element count.
		/// If disconnect is true, will disconnect after reading the value.
		/// </summary>
		/// <param name="pvName"></param>
		/// <param name="type"></param>
		/// <param name="elementCount"></param>
		/// <returns></returns>
		public static async Task<EndPointStatus> CagetAsync(String pvName, System.Type type, int elementCount, Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback? readCallback)
		{
			EndPointStatus status = EndPointStatus.UnknownError;
            // Starts off with a EndPoint connection to the PV
            var endpoint = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetEpicsCaEndPointID(pvName);
			status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync(endpoint, readCallback);
		
			return status;
		}

		public static async Task<EndPointStatus> CagetControlLongMetaDataAsyncInt(string name, Type dataType,  Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCtrlLongCallback? readCallback)
		{
			EndPointID endPointID = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetEpicsCaEndPointID(name);
			return await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.GetMetadataAsync(endPointID, dataType, readCallback);
		}

	}
}
