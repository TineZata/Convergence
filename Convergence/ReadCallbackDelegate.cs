using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ReadCallbackArgs
    {
        public IntPtr usr;
        public IntPtr chid;
#if LP64
      public long type ;
      public long count ;
#else
        public int type;
        public int count;
#endif
        public readonly IntPtr dbr;
        public int status;
    }

    public class ReadCallbackDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadCallback(ReadCallbackArgs data);
    }
}
