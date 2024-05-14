using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{
		public static bool Disconnect(String pvName)
		{
			return ConvergeOnEPICSChannelAccess.Hub.Disconnect(Convergence.IO.EPICS.CA.Helpers.GetEndPointID(pvName));
		}
	}
}
