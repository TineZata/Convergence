﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
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

    [StructLayout(LayoutKind.Sequential)]
    public struct ConnectionEventCallbackArgs
    {
        public const int CA_OP_CONN_UP = 6;
        public const int CA_OP_CONN_DOWN = 7;
        public nint chid;  /* channel id */
        public int op;    /* one of CA_OP_CONN_UP or CA_OP_CONN_DOWN */
    }

    public struct ExceptionHandlerEventArgs
    {
        public nint usr; // User argument supplied when installed
        public nint chid; // Channel id (may be NULL)
        public int type; // Type requested
        public int count; // Count requested
        public nint addr; // User's address to write results of CA_OP_GET 
        public int stat; // Channel access ECA_XXXX status code
        public int op; // CA_OP_GET, CA_OP_PUT, ..., CA_OP_OTHER
        public nint ctx; // Character string containing context info
        public nint pFile; // Source file name (may be NULL)
        public short lineNo; // Source file line number (may be zero)
        public string? Message => Marshal.PtrToStringAnsi(ctx);
        public string RequestInfo => $"Requested {count} elements of type {type}";
    }

    public class EventCallbackDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadCallback(EventCallbackArgs data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void WriteCallback(EventCallbackArgs data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MonitorCallback(EventCallbackArgs data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConnectCallback(ConnectionEventCallbackArgs args);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void ExceptionHandlerCallback(ExceptionHandlerEventArgs args);
    }
}
