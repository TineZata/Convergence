using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{
		public static async Task<bool> DisconnectAsync(String pvName)
		{
			return await ConvergeOnEPICSChannelAccess.Hub.DisconnectAsync(Convergence.IO.EPICS.CA.Helpers.GetEndPointID(pvName));
		}
	}
}
