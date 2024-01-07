﻿//
// DllFunctions.cs
//

using System;
using System.Runtime.InteropServices;
using FluentAssertions;

namespace Convergence.IO.EPICS
{

    internal static class ChannelAccessDLLWrapper
    {

        //
        // Note that there's no need to specify a '.dll' extension.
        // In fact, the runtime adds a '.dll' extension on Windows,
        // and a '.so' extension on Linux, so this P/Invoke code
        // should 'just work' on both Windows and Linux ...
        //
        // https://docs.microsoft.com/en-us/dotnet/standard/native-interop/cross-platform
        // To facilitate simpler cross platform P/Invoke code, the runtime adds the canonical
        // shared library extension (.dll, .so or .dylib) to native library names.
        // On Linux and macOS, the runtime will also try prepending lib. These library name variations
        // are automatically searched when you use APIs that load unmanaged libraries (eg, DllImportAttribute).
        //

        //
        // NOTE : When this assembly is published as a Nuget package,
        // required DLL's have to be located in the same directory as the assembly.
        //
        // Note that the 'ca' DLL (and also 'com.dll') have to be configured
        // in the Project Properties to be 'copied to the Output directory'.
        //
        // ??? The DLL's that we link to are the ones in the root, but copies
        // of various alternative DLLs are kept in the '_DLL' directories
        // so that they can be copied to the root for testing ...
        //

        //
        // Hmm, this static constructor doesn't seem to get called
        // under all circumstances, eg from an XUnit test ???
        // That was observed a few times, but these days it *does*
        // seem to get called, so maybe that was just bad luck :)
        //

        static ChannelAccessDLLWrapper()
        {
        }

        //
        // Exporting from a DLL Using __declspec(dllexport)
        // https://docs.microsoft.com/en-us/cpp/build/exporting-from-a-dll-using-declspec-dllexport?view=msvc-170
        //

        //
        // Aha, if we specify just the name of the DLL then the 'LoadLibrary' call
        // that P/Invoke uses, will search in the usual places to find the library.
        // BUT ALTERNATIVELY WE COULD SPECIFY AN ABSOLUTE PATH TO SEARCH !!!
        // AND THAT COULD BE A DIFFERENT DIRECTORY FOR DEBUG VS RELEASE !!!
        //
        // Note that the string specified in the [DllImport] attribute has to be constant,
        // so you can't specify the DLL location dynamically.
        //
        // HOWEVER - if a DLL named 'ca' has already been loaded, eg with a previous call
        // to 'LoadLibrary', then P/Invoke will not try to load that library again.
        // So, we could call 'LoadLibrary' for all our required DLL's, specifying
        // an absolute path that could be whatever we want at runtime, in our static constructor.
        // Then subsequent calls to P/Invoke will use the DLL's we've loaded !!!
        // DEFINITELY WORTH A TRY.
        // 

        private const string CA_DLL_NAME = ("CA");

        //
        // Most of the API calls return an integer 'ECA' code.
        //

        // -------
        // CONTEXT
        // -------

        public static IntPtr ca_context_create()
        {
            return new(ca_context_create());
            // This function should be called once from each thread
            // prior to making any of the other Channel Access calls.
            // Returns :
            //   ECA_NORMAL      - Normal successful completion
            //   ECA_ALLOCMEM    - Failed, unable to allocate space in pool
            //   ECA_NOTTHREADED - Current thread is already a member
            //                     of a non-preemptive callback CA context
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_context_create
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_current_context
            static extern IntPtr ca_context_create();
        }

        // Needed when you invoke a 'ca_' function on a worker thread

        public static IntPtr ca_current_context()
        {
            return new(ca_current_context());
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_current_context
            static extern IntPtr ca_current_context();
        }

        public static EcaType ca_attach_context(this IntPtr context)
        {
            // Try Parse Enum of type <EcaType> from Int32 return by (ca_attach_context(context))
            if (Enum.TryParse<EcaType>(ca_attach_context(context).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_attach_context: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_attach_context
            static extern Int32 ca_attach_context(IntPtr pClientContext);
        }

        public static void ca_detach_context()
        {
            ca_detach_context();
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_detach_context
            static extern void ca_detach_context();
        }

        public static void ca_context_destroy()
        {
            ca_context_destroy();
            [DllImport(CA_DLL_NAME)]
            // Shut down the calling thread's channel access client context and free any resources allocated.
            // ??? Not necessary on Windows, as the resources used by the client library such as sockets
            // and allocated memory are automatically released by the system when the process exits ??
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_context_destroy
            static extern void ca_context_destroy();
        }

        // -------
        // CHANNEL
        // -------

        public static EcaType ca_create_channel(
          string channelName,
          ConnectionCallback? connectionCallback,
          int tagValue,       // can be fetched later by ca_puser() ; passed in ConnectCallback
          UInt32? priority = null // priority level in the server 0 - 100, default is 0
        )
        {
            if (Enum.TryParse<EcaType>(ca_create_channel(
                channelName, 
                connectionCallback, 
                (System.IntPtr)tagValue,
                priority ?? ChannelAccessConstants.CA_PRIORITY_DEFAULT,
                out var pChannel
            ).ToString(), out EcaType result
            ))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_create_channel: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Creates a CA channel. Establishes and maintains a virtual circuit between the caller's application
            // and a named process variable in a CA server. Each call to ca_create_channel() allocates resources
            // in the CA client library and potentially also a CA server. Use 'ca_clear_channel()' to release
            // these resources.
            // Returns
            //   ECA_NORMAL   - Normal successful completion
            //   ECA_BADTYPE  - Invalid DBR_XXXX type ???????????????
            //   ECA_STRTOBIG - Unusually large string
            //   ECA_ALLOCMEM - Unable to allocate memory
            // **** If the unmanaged function stores the delegate to use after the call completes,
            // you must manually prevent garbage collection until the unmanaged function finishes with the delegate.
            // For more information, see the HandleRef Sample and GCHandle Sample.
            // https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-a-delegate-as-a-callback-method
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_create_channel
            static extern Int32 ca_create_channel(
              string pChanName,
              ConnectionCallback? pConnStateCallback,
              IntPtr pUserPrivate, // can be fetched later by ca_puser() ; passed in 'ConnectCallback'
              UInt32 priority,     // priority level in the server 0 - 100 // put in SETTINGS ???
              out IntPtr pChanID // was 'ref'
            );
        }

        public static EcaType ca_clear_channel(IntPtr pChanID)
        {
            if (Enum.TryParse<EcaType>(ca_clear_channel(pChanID).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_clear_channel: Unable to cast EcaType from Int32");
            }
            
            [DllImport(CA_DLL_NAME)]
            // Shutdown and reclaim resources associated with
            // a channel created by ca_create_channel().
            // Returns
            //   ECA_NORMAL - Normal successful completion
            //   ECA_BADCHID - Corrupted CHID
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_clear_channel
            static extern Int32 ca_clear_channel(IntPtr pChanID);
        }

        // -------------
        // CHANNEL STATE
        // -------------

        public static ChannelState ca_state(IntPtr pChanID)
        {
            return ca_state(pChanID);
            [DllImport(CA_DLL_NAME)]
            // Returns an enumerated type indicating the current state of the specified IO channel.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_state
            static extern ChannelState ca_state(IntPtr pChanID);
        }

        public static string ca_host_name(IntPtr pChanID)
        {
            return Marshal.PtrToStringAnsi(
              ca_host_name(pChanID)
            )!;
            // NOTE : THIS IS NOT THREAD SAFE !!!!
            // BETTER TO USE 'ca_get_host_name' !!!!!!
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_host_name
            static extern IntPtr ca_host_name(IntPtr pChanID);
            // [DllImport(CA_DLL_path)]
            // static extern uint ca_get_host_name ( IntPtr pChanID, IntPtr pBuffer, uint nBufferBytesAllocated ) ;
        }

        // -----------
        // GET AND PUT 
        // -----------

        public unsafe static EcaType ca_array_get(
          this IntPtr pChanID,
          DbRecordRequestType dbrType,
          int nElementsOfThatTypeWanted,
          void* pMemoryAllocatedToHoldDbrStruct
        )
        {
            if (Enum.TryParse<EcaType>(ca_array_get(
              (short)dbrType,
              (uint)nElementsOfThatTypeWanted,
              pChanID,
              (System.IntPtr)pMemoryAllocatedToHoldDbrStruct
            ).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_array_get: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Read a scalar or array value from a process variable.
            // The returned channel value can't be assumed to be stable in the application supplied buffer
            // until after ECA_NORMAL is returned from ca_pend_io().
            // All get requests are accumulated (buffered) and not forwarded to the IOC until one of ca_flush_io(),
            // ca_pend_io(), ca_pend_event(), or ca_sg_block() are called. This allows several requests
            // to be efficiently sent over the network in one message.
            // Returns
            //  ECA_NORMAL     - Normal successful completion
            //  ECA_BADTYPE    - Invalid DBR_XXXX type
            //  ECA_BADCHID    - Corrupted CHID
            //  ECA_BADCOUNT   - Requested count larger than native element count
            //  ECA_GETFAIL    - A local database get failed
            //  ECA_NORDACCESS - Read access denied
            //  ECA_ALLOCMEM   - Unable to allocate memory
            //  ECA_DISCONN    - Channel is disconnected
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_array_get
            static extern int ca_array_get(
              Int16 type,
              UInt32 count,
              IntPtr pChanID,
              IntPtr pValue
            );
        }

        public static EcaType ca_array_get_callback(
          this IntPtr pChanID,
          DbRecordRequestType type,
          int nElementsWanted,
          ValueUpdateCallback valueUpdateCallBack,
          IntPtr userArg
        )
        {
            if (Enum.TryParse<EcaType>(ca_array_get_callback(
              (Int16)type,
              (UInt32)nElementsWanted,
              pChanID,
              valueUpdateCallBack,
              userArg
            ).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_array_get_callback: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Read a scalar or array value from a process variable.
            // A value is read from the channel and then the user's callback is invoked with a pointer to the retrieved value. 
            // Note that ca_pend_io() will not block for the delivery of values requested by ca_get_callback().
            // If the channel disconnects before a ca_get_callback() request can be completed, then the client's
            // callback function is called with failure status.
            // Returns
            //  ECA_NORMAL     - Normal successful completion
            //  ECA_BADTYPE    - Invalid DBR_XXXX type
            //  ECA_BADCHID    - Corrupted CHID
            //  ECA_BADCOUNT   - Requested count larger than native element count
            //  ECA_GETFAIL    - A local database get failed
            //  ECA_NORDACCESS - Read access denied
            //  ECA_ALLOCMEM   - Unable to allocate memory
            //  ECA_DISCONN    - Channel is disconnected
            // **** If the unmanaged function stores the delegate to use after the call completes,
            // you must manually prevent garbage collection until the unmanaged function finishes with the delegate.
            // For more information, see the HandleRef Sample and GCHandle Sample.
            // https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-a-delegate-as-a-callback-method
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_array_get_callback
            static extern int ca_array_get_callback(
              Int16 type,
              UInt32 count,
              IntPtr pChanID,
              ValueUpdateCallback pEventCallBack,
              IntPtr userArg
            );
        }

        // NOTE : ALWAYS FOLLOW THIS WITH 'ca_flush_io()'

        // NOTE THAT 'count' SHOULD BE NO GREATER THAN
        // THE AVAILABLE NUMBER OF ELEMENTS ...

        public static unsafe EcaType ca_array_put(
          this IntPtr pChanID,
          DbRecordRequestType dbrType,
          int nElementsOfThatTypeWanted,
          void* pValueToWrite // New channel value is copied from here
        )
        {
            if (Enum.TryParse<EcaType>(ca_array_put(
              (short)dbrType,
              (uint)nElementsOfThatTypeWanted,
              pChanID,
              (IntPtr)pValueToWrite   // New channel value is copied from here
            ).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_array_put: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Write a scalar or array value to a process variable.
            // The client will receive no response unless the request can not be fulfilled in the server.
            // Returns
            //   ECA_NORMAL     - Normal successful completion
            //   ECA_BADCHID    - Corrupted CHID
            //   ECA_BADTYPE    - Invalid DBR_XXXX type
            //   ECA_BADCOUNT   - Requested count larger than native element count
            //   ECA_STRTOBIG   - Unusually large string supplied
            //   ECA_NOWTACCESS - Write access denied
            //   ECA_ALLOCMEM   - Unable to allocate memory
            //   ECA_DISCONN    - Channel is disconnected
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_array_put
            static extern Int32 ca_array_put(
              Int16 type,
              UInt32 count,
              IntPtr pchanID,
              IntPtr pValue   // New channel value is copied from here
            );
        }

        // BETTER NAME : InitiateWrite_WithCallback
        // NOTE : ALWAYS FOLLOW THIS WITH 'ca_flush_io()'

        // NOTE THAT 'count' SHOULD BE NO GREATER THAN
        // THE AVAILABLE NUMBER OF ELEMENTS ...

        public unsafe static EcaType ca_array_put_callback(
          this IntPtr pChanID,
          DbRecordRequestType dbrType,
          int nElementsOfThatTypeWanted,
          void* pValueToWrite,       // New channel value is copied from here
          ValueUpdateCallback valueUpdateCallback, // Event will be raised when successful write is confirmed
          int userArg
        )
        {
            if (Enum.TryParse<EcaType>(ca_array_put_callback(
              (short)dbrType,
              (uint)nElementsOfThatTypeWanted,
              pChanID,
              (IntPtr)pValueToWrite, // New value is copied from here
              valueUpdateCallback, // Event will be raised when successful write is confirmed
              (IntPtr)userArg
            ).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_array_put_callback: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // This routine functions identically to the original 'ca_put' request 
            // with the addition of a callback to the user-supplied function 
            // after the initiated write operation, and all actions resulting
            // from the initiating write operation, have completed.
            // If unsuccessful, the callback function is invoked indicating failure status.
            // If successful, the status indicates ECA_NORMAL but that's all we get,
            // the callback supplies a 'null' pointer for the dbRecord.
            // Returns
            //   ECA_NORMAL     - Normal successful completion
            //   ECA_BADCHID    - Corrupted CHID
            //   ECA_BADTYPE    - Invalid DBR_XXXX type
            //   ECA_BADCOUNT   - Requested count larger than native element count
            //   ECA_STRTOBIG   - Unusually large string supplied
            //   ECA_NOWTACCESS - Write access denied
            //   ECA_ALLOCMEM   - Unable to allocate memory
            //   ECA_DISCONN    - Channel is disconnected
            // **** NOTE THAT AFTER CALLING THIS FUNCTION IT'S NECESSARY TO CALL 'ca_flush_io'
            // OTHERWISE THE CALLBACK WON'T BE ACTIVATED UNTIL THE NEXT 'AUTOMATIC' FLUSH HAPPENS
            // WHICH SEEMS TO BE AFTER AN INTERVAL OF ABOUT 30 SECS !!!
            // **** If the unmanaged function stores the delegate to use after the call completes,
            // you must manually prevent garbage collection until the unmanaged function finishes with the delegate.
            // For more information, see the HandleRef Sample and GCHandle Sample.
            // https://docs.microsoft.com/en-us/dotnet/framework/interop/marshaling-a-delegate-as-a-callback-method
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_array_put_callback
            static extern Int32 ca_array_put_callback(
              Int16 type,
              UInt32 count,
              IntPtr pchanID,
              IntPtr pValue,         // New value is copied from here
              ValueUpdateCallback pEventCallBack, // Event will be raised when successful write is confirmed
              IntPtr userArg
            );
        }

        // -------------
        // SUBSCRIPTIONS
        // -------------

        public unsafe static IntPtr ca_create_subscription(
          this IntPtr pChanID,
          DbRecordRequestType dbrType,
          int count,
          WhichFieldsToMonitor whichFieldsToMonitor,
          ValueUpdateCallback valueUpdateCallback,
          int userArg
        )
        {
            if (Enum.TryParse<EcaType>(ca_create_subscription(
              (short)dbrType,  // DBR_xxx
              (uint)count,
              pChanID,
              (uint)whichFieldsToMonitor,
              valueUpdateCallback,
              (System.IntPtr)userArg,
              out IntPtr pEvid
            ).ToString(), out EcaType result))
            {
                return pEvid;
            }
            else
            {
                throw new InvalidCastException("ca_create_subscription: Unable to cast EcaType from Int32");
            }
            return pEvid;
            [DllImport(CA_DLL_NAME)]
            // Register a state change subscription and specify a callback function
            // to be invoked whenever the process variable undergoes significant state changes.
            // A significant change can be a change in the process variable's value, alarm status, or alarm severity.  ****** ????
            // The 'mask' argument has bits set for each of the event trigger types requested.
            // The event trigger mask must be a bitwise or of one or more of the following constants :
            //   DBE_VALUE                - Trigger events when the channel value exceeds the monitor dead band
            //   DBE_ARCHIVE (or DBE_LOG) - Trigger events when the channel value exceeds the archival dead band
            //   DBE_ALARM                - Trigger events when the channel alarm state changes
            //   DBE_PROPERTY             - Trigger events when a channel property changes.
            // Returns
            //   ECA_NORMAL   - Normal successful completion
            //   ECA_BADCHID  - Corrupted CHID
            //   ECA_BADTYPE  - Invalid DBR_XXXX type
            //   ECA_ALLOCMEM - Unable to allocate memory
            //   ECA_ADDFAIL  - A local database event add failed
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_add_event (???)
            static extern Int32 ca_create_subscription(
              Int16 dbrType,  // DBR_xxx
              UInt32 count,
              IntPtr pChanID,
              uint mask,
              ValueUpdateCallback pEventCallBack,
              IntPtr userArg,
              out IntPtr pEvid
            );
        }

        public static void ca_clear_subscription(this SubscriptionHandle subscription)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            subscription.IsValidHandle.Should().BeTrue();
            ca_clear_subscription(subscription).VerifyEcaSuccess();
            [DllImport(CA_DLL_NAME)]
            // Cancel a subscription.
            // Returns
            //   ECA_NORMAL   - Normal successful completion
            //   ECA_BADCHID  - Corrupted CHID
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_clear_subscription
            static extern Int32 ca_clear_subscription(IntPtr pEvid);
        }

        // ------------
        // FLUSH_IO etc
        // ------------

        public static void ca_flush_io()
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            ca_flush_io().VerifyEcaSuccess();
            [DllImport(CA_DLL_NAME)]
            // Flush outstanding IO requests to the server.
            // Returns
            //   ECA_NORMAL - Normal successful completion
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_flush_io
            static extern Int32 ca_flush_io();
        }

        // BETTER NAME : AllReadRequestsCompleted_AfterBlockingWaitOfAtMost

        private static volatile bool g_ca_pend_io_call_in_progress = false;

        public static bool ca_pend_io(double timeOut_secs_zeroMeansInfinite)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            g_ca_pend_io_call_in_progress.Should().BeFalse();
            g_ca_pend_io_call_in_progress = true;
            ApiCallResult ecaResult = ca_pend_io(timeOut_secs_zeroMeansInfinite);
            g_ca_pend_io_call_in_progress = false;
            if (ecaResult.MessageNumber == EcaType.ECA_MESSAGE_TIMEOUT)
            {
                return false;
            }
            // else if ( ecaResult.MessageNumber == EcaMessage.ECA_MESSAGE_EVDISALLOW )
            // {
            //   return false ;
            // }
            else
            {
                //if ( ecaResult.MessageNumber == EcaMessage.ECA_MESSAGE_EVDISALLOW )
                //{
                //  // Can put a breakpoint here ...
                //  int x = 0 ;
                //}
                ecaResult.VerifySuccess();
                return true;
            }
            [DllImport(CA_DLL_NAME)]
            // Flushes the send buffer and then blocks until outstanding ca_get() requests complete.
            // Returns
            //   ECA_NORMAL - Normal successful completion
            //   ECA_TIMEOUT - Selected IO requests didn't complete before specified timeout
            //   ECA_EVDISALLOW - Function inappropriate for use within an event handler
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_pend_io
            static extern Int32 ca_pend_io(double timeOut_secs_zeroMeansInfinite);
        }

        // Returns true if all I/O requests are complete

        // BETTER NAME : AllReadRequestsHaveCompleted
        // This is not called, and may be dangerous ??

        public static bool ca_test_io()
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            ApiCallResult ecaResult = ca_test_io();
            var messageCode = ecaResult.MessageNumber;
            return messageCode switch
            {
                EcaType.ECA_MESSAGE_IODONE => true,
                EcaType.ECA_MESSAGE_IOINPROGRESS => false,
                _ => throw new UnexpectedConditionException(
                       ecaResult.GetExceptionMessage("ca_test_io")
                     )
            };
            [DllImport(CA_DLL_NAME)]
            // Tests to see if all ca_get() requests are complete.
            // Returns
            //   ECA_IODONE       - All IO operations completed
            //   ECA_IOINPROGRESS - IO operations still in progress
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_test_io
            static extern Int32 ca_test_io();
        }

        // BETTER NAME : BlockWaitingForMonitorEvents 
        // This is not called, and may be dangerous ??

        public static void ca_pend_event(double nSecsToBlock_zeroMeansInfinite)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            ca_pend_event(nSecsToBlock_zeroMeansInfinite).VerifyEcaSuccess();
            [DllImport(CA_DLL_NAME)]
            // Wait for channel subscription events and call the functions
            // specified with add_event when events occur.
            // When ca_pend_event is invoked the send buffer is flushed and CA background activity is processed for TIMEOUT seconds.
            // The ca_pend_event function will not return before the specified time-out expires
            // and all unfinished channel access labor has been processed
            // Timeout of 0 gets you an infinite timeout is assumed.
            //   ECA_NORMAL - Normal successful completion
            //   ECA_TIMEOUT - Selected IO requests didn't complete before specified timeout
            //   ECA_EVDISALLOW - Function inappropriate for use within an event handler
            // ??? Doc says : ca_pend_event returns ECA_TIMEOUT when successful. This behavior probably isn't intuitive,
            // but it is preserved to insure backwards compatibility. WTF ??? !!!
            // https://epics.anl.gov/base/R3-14/10-docs/CAref.html#L3249
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_pend_event
            static extern Int32 ca_pend_event(double timeOut_secs_zeroMeansInfinite);
        }

        // -------
        // QUERIES
        // -------

        // These query functions don't require a message to the server,
        // once a connection has been establised initially.

        // This tells us the *max* number of elements that the server will deal with.

        public static int ca_element_count(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return (int)ca_element_count(channel);
            [DllImport(CA_DLL_NAME)]
            // Returns the maximum array element count in the server for the specified channel.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_element_count
            static extern UInt32 ca_element_count(IntPtr pChanID);
        }

        public static DbFieldType ca_field_type(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return ca_field_type(channel).AsEnumType<DbFieldType>();
            [DllImport(CA_DLL_NAME)]
            // Returns the native 'field type' in the server of the process variable.
            // The returned code will be one of the DBF_ values, or 'DBF_NO_ACCESS'
            // if the channel is disconnected.
            // Could the field type have changed, on a reconnect ? Detect that !!!
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_field_type
            static extern Int16 ca_field_type(IntPtr pChanID);
        }

        public static bool ca_has_invalid_field_type(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return ca_field_type(channel) < 0;
            [DllImport(CA_DLL_NAME)]
            // Returns the native 'field type' in the server of the process variable.
            // The returned code will be one of the DBF_ values, or 'DBF_NO_ACCESS'
            // if the channel is disconnected.
            // Could the field type have changed, on a reconnect ? Detect that !!!
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_field_type
            static extern Int16 ca_field_type(IntPtr pChanID);
        }

        public static bool ca_field_type_reports_no_access(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return ca_field_type(channel) == ChannelAccessConstants.DBF_NO_ACCESS;
            [DllImport(CA_DLL_NAME)]
            // Returns the native 'field type' in the server of the process variable.
            // The returned code will be one of the DBF_ values, or 'DBF_NO_ACCESS'
            // if the channel is disconnected.
            // Could the field type have changed, on a reconnect ? Detect that !!!
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_field_type
            static extern Int16 ca_field_type(IntPtr pChanID);
        }

        // If we have write access, do we always have 'read' access ??
        // Might have a PV where writing anything triggers an action,
        // but the value itself isn't readable ?
        // In that case, we probably need a special FieldType ???

        public static bool ca_read_access(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return ca_read_access(channel) != 0;
            [DllImport(CA_DLL_NAME)]
            // Returns boolean true if the client currently has read access
            // to the specified channel and boolean false otherwise.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_read_access
            static extern Int32 ca_read_access(IntPtr pChanID);
        }

        public static bool ca_write_access(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return ca_write_access(channel) != 0;
            [DllImport(CA_DLL_NAME)]
            // Returns boolean true if the client currently has write access
            // to the specified channel and boolean false otherwise.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_write_access
            static extern Int32 ca_write_access(IntPtr pChanID);
        }

        public static string ca_name(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return Marshal.PtrToStringAnsi(
              ca_name(channel.Value)
            )!;
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_name
            static extern IntPtr ca_name(IntPtr pChanID);
        }

        // Get the 'user info tag' associated with the channel

        public static int ca_puser(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return (int)ca_puser(channel);
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_puser
            static extern IntPtr ca_puser(IntPtr chan);
        }

        // Set the 'user info tag' associated with the channel

        public static void ca_set_puser(this ChannelHandle channel, int userInfo)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            ca_set_puser(channel, (nint)userInfo);
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_set_puser
            static extern void ca_set_puser(IntPtr chan, IntPtr puser);
        }

        // Get the 'beacon period' for the channel.
        // Hmm, this might be useful to know, however the value comes back as a junk value
        // even if we wait until the channel has been successfully connected-to ...

        public static double ca_beacon_period(this ChannelHandle channel)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return ca_beacon_period(channel);
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_beacon_period
            static extern double ca_beacon_period(IntPtr chan);
        }

        // Replace the default 'exception handler'

        public static void ca_add_exception_event(
          ExceptionHandlerCallback pExceptionHandlerCallBack,
          nint userArg
        )
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            ca_add_exception_event(
              pExceptionHandlerCallBack,
              userArg
            ).VerifyEcaSuccess();
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_add_exception_event
            // This changes 'ca_exception_func' and 'ca_exception_arg'.
            // Called via 'ca_client_context::exception'
            static extern Int32 ca_add_exception_event(
              ExceptionHandlerCallback pEventCallBack,
              IntPtr userArg // 'exception_handler_args'
            );
        }

        public static void ca_replace_printf_handler(PrintfCallback printfCallback)
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            ca_replace_printf_handler(printfCallback).VerifyEcaSuccess();
            // Replace the 'printf' handler ... ???
            // This is problematic because 'ca_printf_func' 
            // needs to be a pointer to a function with a 'va_list' parameter :
            //   typedef int caPrintfFunc ( const char * pformat, va_list args ) ;
            // https://github.com/dotnet/runtime/issues/9316
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_replace_printf_handler
            // Our callback sets up 'pVPrintfFunc' which gets invoked
            // when 'varArgsPrintFormated' and 'printFormated' are called.
            // BUT there are also many calls to '::printf'.
            // The annoying 'change may be required in your path' message
            // comes from a printf in 'osdProcess.c', line 96.
            // Hmm, maybe safest to find a way of intercepting stdout/stderr calls ?
            [DllImport(CA_DLL_NAME)]
            static extern Int32 ca_replace_printf_handler(
              PrintfCallback ca_printf_func
            );
        }

        public static string ca_version()
        {
            EnsureCurrentThreadIsAttachedToChannelsHubContext();
            return Marshal.PtrToStringAnsi(
              ca_version()
            )!;
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_version
            static extern IntPtr ca_version();
        }

    }

}
