using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    public class EventCallbackDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadCallback<T>(T result);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WriteCallback<T>(T result);
    }
}
