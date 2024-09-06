using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
	public class CagetValueSnapshot
	{
		public EndPointStatus Status { get; set; }
		public object? Value { get; set; }
	}
}
