using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    public enum EndPointStatus
    {
        Disconnected,
        InvalidName,
        InvalidDataType,
        TimedOut,
        NoReadAccess,
        NoWriteAccess,
        UnknownError,
        InvaildProtocol,
		ReadFailed,
		Okay,
    }
}
