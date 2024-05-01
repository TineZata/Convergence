
using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using static Convergence.IO.EPICS.CA.EventCallbackDelegate;

namespace Convergence.IO.EPICS.CA
{

    internal static class ChannelAccessWrapper
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

        static ChannelAccessWrapper()
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

        private const string CA_DLL_NAME = "IO\\EPICS\\CA\\CA";

        //
        // Most of the API calls return an integer 'ECA' code.
        //

        // -------
        // CONTEXT
        // -------

        public static nint CurrentContext { get; private set; }

        private static void set_current_context()
        {
            var context = ca_current_context();
            // Check if CurrentContext has been created
            if (CurrentContext == nint.Zero)
            {
                context.ca_attach_context();
                // Check if CurrentContext has been created
                CurrentContext = ca_current_context();
                try
                {
                    CurrentContext.Should().Be(context);
                }
                catch (Exception ex)
                {
                    throw new Exception("CurrentContext is not equal to context", ex);
                }
            }
            // else if CurrentContext is not nint.Zero but not equal to context
            else if (CurrentContext != ca_current_context())
            {
                // Destroy the CurrentContext
                ca_context_destroy();
                // Get a new context
                CurrentContext = ca_current_context();
                try
                {
                    CurrentContext.Should().Be(context);
                }
                catch (Exception ex)
                {
                    throw new Exception("CurrentContext is not equal to context", ex);
                }
            }
            else
            {
                // Do nothing
            }
        }

        /// <summary>
        /// This function should be called once from each thread
        /// prior to making any of the other Channel Access calls.
        /// </summary>
        /// <param name="preemptive"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static EcaType ca_context_create(PreemptiveCallbacks preemptive)
        {
            if (Enum.TryParse<EcaType>(CA_EXTRACT_MSG_NO(ca_context_create(preemptive)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_context_create: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // 
            // Returns :
            //   ECA_NORMAL      - Normal successful completion
            //   ECA_ALLOCMEM    - Failed, unable to allocate space in pool
            //   ECA_NOTTHREADED - Current thread is already a member
            //                     of a non-preemptive callback CA context
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_context_create

            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_current_context
            static extern int ca_context_create(PreemptiveCallbacks p);

        }

        // Needed when you invoke a 'ca_' function on a worker thread

        public static nint ca_current_context()
        {
            return new(ca_current_context());
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_current_context
            static extern nint ca_current_context();
        }

        public static EcaType ca_attach_context(this nint context)
        {
            // Try Parse Enum of type <EcaType> from Int32 return by (ca_attach_context(context))
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_attach_context(context)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_attach_context: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_attach_context
            static extern int ca_attach_context(nint pClientContext);
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
          ConnectCallback? connectionCallback,
          out nint pChannel
        )
        {
            set_current_context();
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_create_channel(
                channelName,
                connectionCallback,
                nint.Zero,
                ChannelAccessConstants.CA_PRIORITY_DEFAULT,
                out pChannel
            )).ToString(), out EcaType result
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
            static extern int ca_create_channel(
              string pChanName,
              ConnectCallback? pConnStateCallback,
              nint pUserPrivate, // can be fetched later by ca_puser() ; passed in 'ConnectCallback'
              uint priority,     // priority level in the server 0 - 100 // put in SETTINGS ???
              out nint pChannel // was 'ref'
            );
        }

        public static EcaType ca_clear_channel(nint pChanID)
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_clear_channel(pChanID)).ToString(), out EcaType result))
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
            static extern int ca_clear_channel(nint pChanID);
        }

        // -------------
        // CHANNEL STATE
        // -------------

        public static ChannelState ca_state(nint pChanID)
        {
            return ca_state(pChanID);
            [DllImport(CA_DLL_NAME)]
            // Returns an enumerated type indicating the current state of the specified IO channel.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_state
            static extern ChannelState ca_state(nint pChanID);
        }

        public static string ca_host_name(nint pChanID)
        {
            return Marshal.PtrToStringAnsi(
              ca_host_name(pChanID)
            )!;
            // NOTE : THIS IS NOT THREAD SAFE !!!!
            // BETTER TO USE 'ca_get_host_name' !!!!!!
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_host_name
            static extern nint ca_host_name(nint pChanID);
            // [DllImport(CA_DLL_path)]
            // static extern uint ca_get_host_name ( nint pChanID, nint pBuffer, uint nBufferBytesAllocated ) ;
        }

        // -----------
        // GET AND PUT 
        // -----------

        public unsafe static EcaType ca_array_get(
          this nint pChanID,
          DbFieldType dbrType,
          int nElementsOfThatTypeWanted,
          nint pMemoryAllocatedToHoldDbrStruct
        )
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_array_get(
              (short)dbrType,
              (uint)nElementsOfThatTypeWanted,
              pChanID,
              pMemoryAllocatedToHoldDbrStruct
            )).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                return EcaType.ECA_GETFAIL;
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
              short type,
              uint count,
              nint pChanID,
              nint pValue
            );
        }

        public static EcaType ca_array_get_callback(
          this nint pChanID,
          DbFieldType type,
          int nElements,
          ReadCallback valueUpdateCallBack
        )
        {
            // Check that the channel is connected
            if (ca_state(pChanID) != ChannelState.CurrentlyConnected) return EcaType.ECA_DISCONN;
            // Check that write access is allowed
            if (!pChanID.ca_read_access()) return EcaType.ECA_NORDACCESS;
            // Check that the data type is valid
            var returnedType = pChanID.ca_field_type();
            // If user defined type is DbFieldType.DBF_FLOAT_f32 the the return type can either be DBR_FLOAT or DBR_DOUBLE.
            // Actually I've never observer a float type being returned, however this is possible according to the EPICS documentation when 
            // PREC is set to the correct value.
            bool isFloat = type == DbFieldType.DBF_FLOAT_f32 && (returnedType == (short)DbFieldType.DBF_FLOAT_f32 || returnedType == (short)DbFieldType.DBF_DOUBLE_f64);
            if ((short)type != returnedType && !isFloat)
                return EcaType.ECA_BADTYPE;
            // assign userArg to ca_puser(pChanID)
            var userArg = pChanID.ca_puser();
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(
                ca_array_get_callback(
                  (short)type,
                  (uint)nElements,
                  pChanID,
                  valueUpdateCallBack,
                  userArg)
                ).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                return EcaType.ECA_GETFAIL;
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
              short type,
              uint count,
              nint pChanID,
              ReadCallback pEventCallBack,
              nint userArg
            );
        }

        // NOTE : ALWAYS FOLLOW THIS WITH 'ca_flush_io()'

        // NOTE THAT 'count' SHOULD BE NO GREATER THAN
        // THE AVAILABLE NUMBER OF ELEMENTS ...

        public static unsafe EcaType ca_array_put(
          this nint pChanID,
          DbFieldType dbrType,
          int nElements,
          nint pValueToWrite // New channel value is copied from here
        )
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_array_put(
              (short)dbrType,
              (uint)nElements,
              pChanID,
              pValueToWrite   // New channel value is copied from here
            )).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                return EcaType.ECA_PUTFAIL;
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
            static extern int ca_array_put(
              short type,
              uint count,
              nint pchanID,
              nint pValue   // New channel value is copied from here
            );
        }

        // BETTER NAME : InitiateWrite_WithCallback
        // NOTE : ALWAYS FOLLOW THIS WITH 'ca_flush_io()'

        // NOTE THAT 'count' SHOULD BE NO GREATER THAN
        // THE AVAILABLE NUMBER OF ELEMENTS ...

        public unsafe static EcaType ca_array_put_callback(
          this nint pChanID,
          DbFieldType dbrType,
          int nElements,
          nint ptrValueToWrite,
          WriteCallback writeCallback // Event will be raised when successful write is confirmed
        )
        {
            // Check that the channel is connected
            if (ca_state(pChanID) != ChannelState.CurrentlyConnected) return EcaType.ECA_DISCONN;
            // Check that write access is allowed
            if (!pChanID.ca_read_access()) return EcaType.ECA_NORDACCESS;
            // Check that the data type is valid
            var returnedType = pChanID.ca_field_type();
            // assign userArg to ca_puser(pChanID)
            // If user defined type is DbFieldType.DBF_FLOAT_f32 the the return type can either be DBR_FLOAT or DBR_DOUBLE.
            // Actually I've never observer a float type being returned, however this is possible according to the EPICS documentation when 
            // PREC is set to the correct value.
            bool isFloat = dbrType == DbFieldType.DBF_FLOAT_f32 && (returnedType == (short)DbFieldType.DBF_FLOAT_f32 || returnedType == (short)DbFieldType.DBF_DOUBLE_f64);
            if ((short)dbrType != returnedType && !isFloat)
                return EcaType.ECA_BADTYPE;

            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_array_put_callback(
              (short)dbrType,
              (uint)nElements,
              pChanID,
              ptrValueToWrite, // New value is copied from here
              writeCallback, // Event will be raised when successful write is confirmed
              pChanID.ca_puser()
            )).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                return EcaType.ECA_PUTFAIL;
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
            static extern int ca_array_put_callback(
              short type,
              uint count,
              nint pchanID,
              nint pValue,         // New value is copied from here
              WriteCallback pEventCallBack, // Event will be raised when successful write is confirmed
              nint userArg
            );
        }

        // -------------
        // SUBSCRIPTIONS
        // -------------

        public unsafe static EcaType ca_create_subscription(
          this nint pChanID,
          DbFieldType dbrType,
          int count,
          MonitorTypes? whichFieldsToMonitor,
          MonitorCallback valueUpdateCallback,
          out nint pEvid
        )
        {
            pEvid = nint.Zero;
            // Check that the channel is connected
            if (ca_state(pChanID) != ChannelState.CurrentlyConnected) return EcaType.ECA_DISCONN;
            // Check that read access is allowed
            if (!pChanID.ca_read_access()) return EcaType.ECA_NORDACCESS;
            // assign userArg to ca_puser(pChanID)
            var userArg = pChanID.ca_puser();
            whichFieldsToMonitor ??= MonitorTypes.MonitorValField;
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_create_subscription(
              (short)dbrType,
              (uint)count,
              pChanID,
              (uint)whichFieldsToMonitor,
              valueUpdateCallback,
              userArg,
              out pEvid
            )).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                return EcaType.ECA_ADDFAIL;
            }
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
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_add_event
            static extern int ca_create_subscription(
              short dbrType,
              uint count,
              nint pChanID,
              uint mask,
              MonitorCallback pEventCallBack,
              nint userArg,
              out nint pEvid
            );
        }

        public static EcaType ca_clear_subscription(this nint pChanID)
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_clear_subscription(pChanID)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_clear_subscription: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Cancel a subscription.
            // Returns
            //   ECA_NORMAL   - Normal successful completion
            //   ECA_BADCHID  - Corrupted CHID
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_clear_subscription
            static extern int ca_clear_subscription(nint pEvid);
        }

        // ------------
        // FLUSH_IO etc
        // ------------

        public static EcaType ca_flush_io()
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_flush_io()).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_flush_io: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Flush outstanding IO requests to the server.
            // Returns
            //   ECA_NORMAL - Normal successful completion
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_flush_io
            static extern int ca_flush_io();
        }


        public static EcaType ca_pend_io(double timeOut_secs_zeroMeansInfinite)
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_pend_io(timeOut_secs_zeroMeansInfinite)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_pend_io: Unable to cast EcaType from Int32");
            }

            [DllImport(CA_DLL_NAME)]
            // Flushes the send buffer and then blocks until outstanding ca_get() requests complete.
            // Returns
            //   ECA_NORMAL - Normal successful completion
            //   ECA_TIMEOUT - Selected IO requests didn't complete before specified timeout
            //   ECA_EVDISALLOW - Function inappropriate for use within an event handler
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_pend_io
            static extern int ca_pend_io(double timeOut_secs_zeroMeansInfinite);
        }

        public static EcaType ca_test_io()
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_test_io()).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_test_io: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // Tests to see if all ca_get() requests are complete.
            // Returns
            //   ECA_IODONE       - All IO operations completed
            //   ECA_IOINPROGRESS - IO operations still in progress
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_test_io
            static extern int ca_test_io();
        }

        public static EcaType ca_pend_event(double nSecsToBlock_zeroMeansInfinite)
        {

            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_pend_event(nSecsToBlock_zeroMeansInfinite)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_pend_event: Unable to cast EcaType from Int32");
            }
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
            static extern int ca_pend_event(double timeOut_secs_zeroMeansInfinite);
        }

        // -------
        // QUERIES
        // -------

        // These query functions don't require a message to the server,
        // once a connection has been establised initially.

        // This tells us the *max* number of elements that the server will deal with.

        public static uint ca_element_count(this nint pChanID)
        {
            return ca_element_count(pChanID);
            [DllImport(CA_DLL_NAME)]
            // Returns the maximum array element count in the server for the specified channel.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_element_count
            static extern uint ca_element_count(nint pChanID);
        }

        public static short ca_field_type(this nint pChanID)
        {
            return ca_field_type(pChanID);

            [DllImport(CA_DLL_NAME)]
            // Returns the native 'field type' in the server of the process variable.
            // The returned code will be one of the DBF_ values, or 'DBF_NO_ACCESS'
            // if the channel is disconnected.
            // Could the field type have changed, on a reconnect ? Detect that !!!
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_field_type
            static extern short ca_field_type(nint pChanID);
        }

        // If we have write access, do we always have 'read' access ??
        // Might have a PV where writing anything triggers an action,
        // but the value itself isn't readable ?
        // In that case, we probably need a special FieldType ???

        public static bool ca_read_access(this nint pChanID)
        {
            return ca_read_access(pChanID) != 0;
            [DllImport(CA_DLL_NAME)]
            // Returns boolean true if the client currently has read access
            // to the specified channel and boolean false otherwise.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_read_access
            static extern int ca_read_access(nint pChanID);
        }

        public static bool ca_write_access(this nint pChanID)
        {
            return ca_write_access(pChanID) != 0;
            [DllImport(CA_DLL_NAME)]
            // Returns boolean true if the client currently has write access
            // to the specified channel and boolean false otherwise.
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_write_access
            static extern int ca_write_access(nint pChanID);
        }

        public static string ca_name(this nint pChanID)
        {
            return Marshal.PtrToStringAnsi(
              ca_name(pChanID)
            )!;
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_name
            static extern nint ca_name(nint pChanID);
        }

        // Get the 'user info tag' associated with the channel

        public static nint ca_puser(this nint pChanID)
        {
            return (int)ca_puser(pChanID);
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_puser
            static extern nint ca_puser(nint pChanID);
        }

        // Set the 'user info tag' associated with the channel

        public static void ca_set_puser(this nint pChanID, nint userInfo)
        {
            ca_set_puser(pChanID, userInfo);
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_set_puser
            static extern void ca_set_puser(nint chan, nint puser);
        }

        // Get the 'beacon period' for the channel.
        // Hmm, this might be useful to know, however the value comes back as a junk value
        // even if we wait until the channel has been successfully connected-to ...

        public static double ca_beacon_period(this nint pChanID)
        {
            return ca_beacon_period(pChanID);
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_beacon_period
            static extern double ca_beacon_period(nint chan);
        }

        // Replace the default 'exception handler'

        public static EcaType ca_add_exception_event(ExceptionHandlerCallback pExceptionHandlerCallBack, nint userArg)
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(ca_add_exception_event(pExceptionHandlerCallBack, userArg)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_add_exception_event: Unable to cast EcaType from Int32");
            }
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_add_exception_event
            // This changes 'ca_exception_func' and 'ca_exception_arg'.
            // Called via 'ca_client_context::exception'
            static extern int ca_add_exception_event(
              ExceptionHandlerCallback pEventCallBack,
              nint userArg // 'exception_handler_args'
            );
        }

        public static string ca_version()
        {
            return Marshal.PtrToStringAnsi(
              ca_version()
            )!;
            [DllImport(CA_DLL_NAME)]
            // https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_version
            static extern nint ca_version();
        }

        private static int CA_EXTRACT_MSG_NO(int eca_code) => (eca_code & ChannelAccessConstants.CA_M_MSG_NO) >> ChannelAccessConstants.CA_V_MSG_NO;
    }

}
