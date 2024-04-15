using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EventCallbackArgs
    {
        public nint usr;
        public nint chid;
#if LP64
      public long type ;
      public long count ;
#else
        public int type;
        public int count;
#endif
        public readonly nint dbr;
        public int status;
    }

    public class CaEventCallbackDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CaReadCallback(EventCallbackArgs data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CaWriteCallback(EventCallbackArgs data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CaMonitorCallback(EventCallbackArgs data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConnectionCallback(ConnectionStatusChangedEventArgs args);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ExceptionHandlerCallback(ExceptionHandlerEventArgs args);
    }
}
