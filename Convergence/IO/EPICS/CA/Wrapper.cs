using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
	public static partial class Wrapper
	{
		// Private readonly null write callback
		private static readonly Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback? _nullWriteCallback = null;
		// Private readonly null connection changed callback
		private static readonly Convergence.IO.EPICS.CA.EventCallbackDelegate.ConnectCallback? _nullConnectionCallback = null;

		private static EndPointStatus _status = EndPointStatus.Disconnected;
		private static object? _value = null;
	}
}
