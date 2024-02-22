using System;
using System.Runtime.InteropServices;
using Convergence.IO.EPICS;

namespace Convergence.IO
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ConnectionCallback(ConnectionStatusChangedEventArgs args);

    

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ExceptionHandlerCallback(ExceptionHandlerEventArgs args);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void PrintfCallback(string format, ArgIterator va_list);

}
