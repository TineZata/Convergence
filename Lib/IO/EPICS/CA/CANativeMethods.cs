using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Convergence.IO.EPICS.CA.EventCallbackDelegate;

namespace Convergence.IO.EPICS.CA
{
	public static partial class CANativeMethods
	{
		private const string CA_DLL_NAME = "IO\\EPICS\\CA\\CA";

		[DllImport(CA_DLL_NAME)]
		// 
		// Returns :
		//   ECA_NORMAL      - Normal successful completion
		//   ECA_ALLOCMEM    - Failed, unable to allocate space in pool
		//   ECA_NOTTHREADED - Current thread is already a member
		//                     of a non-preemptive callback CA context
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_context_create

		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_current_context
		public static extern int ca_context_create(PreemptiveCallbacks p);


		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_current_context
		public static extern nint ca_current_context();


		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_attach_context
		public static extern int ca_attach_context(nint pClientContext);

		[LibraryImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_detach_context
		public static partial void ca_detach_context();


		[DllImport(CA_DLL_NAME)]
		// Shut down the calling thread's channel access client context and free any resources allocated.
		// ??? Not necessary on Windows, as the resources used by the client library such as sockets
		// and allocated memory are automatically released by the system when the process exits ??
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_context_destroy
		public static extern void ca_context_destroy();

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
		public static extern int ca_create_channel(
				  string pChanName,
				  ConnectCallback? pConnStateCallback,
				  nint pUserPrivate, // can be fetched later by ca_puser() ; passed in 'ConnectCallback'
				  uint priority,     // priority level in the server 0 - 100 // put in SETTINGS ???
				  out nint pChannel // was 'ref'
				);

		[DllImport(CA_DLL_NAME)]
		// Shutdown and reclaim resources associated with
		// a channel created by ca_create_channel().
		// Returns
		//   ECA_NORMAL - Normal successful completion
		//   ECA_BADCHID - Corrupted CHID
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_clear_channel
		public static extern int ca_clear_channel(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// Returns an enumerated type indicating the current state of the specified IO channel.
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_state
		public static extern ChannelState ca_state(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_host_name
		public static extern nint ca_host_name(nint pChanID);

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
		public static extern int ca_array_get(
				  short type,
				  uint count,
				  nint pChanID,
				  nint pValue
				);

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
		public static extern int ca_array_get_callback(
				  short type,
				  uint count,
				  nint pChanID,
				  ReadCallback pEventCallBack,
				  nint userArg
				);

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
		public static extern int ca_array_put(
			  short type,
			  uint count,
			  nint pchanID,
			  nint pValue   // New channel value is copied from here
			);


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
		public static extern int ca_array_put_callback(
			  short type,
			  uint count,
			  nint pchanID,
			  nint pValue,         // New value is copied from here
			  WriteCallback pEventCallBack, // Event will be raised when successful write is confirmed
			  nint userArg
			);

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
			public static extern int ca_create_subscription(
			  short dbrType,
			  uint count,
			  nint pChanID,
			  uint mask,
			  MonitorCallback pEventCallBack,
			  nint userArg,
			  out nint pEvid
			);

		[DllImport(CA_DLL_NAME)]
		// Cancel a subscription.
		// Returns
		//   ECA_NORMAL   - Normal successful completion
		//   ECA_BADCHID  - Corrupted CHID
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_clear_subscription
		public static extern int ca_clear_subscription(nint pEvid);

		[DllImport(CA_DLL_NAME)]
		// Flush outstanding IO requests to the server.
		// Returns
		//   ECA_NORMAL - Normal successful completion
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_flush_io
		public static extern int ca_flush_io();

		[DllImport(CA_DLL_NAME)]
		// Flushes the send buffer and then blocks until outstanding ca_get() requests complete.
		// Returns
		//   ECA_NORMAL - Normal successful completion
		//   ECA_TIMEOUT - Selected IO requests didn't complete before specified timeout
		//   ECA_EVDISALLOW - Function inappropriate for use within an event handler
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_pend_io
		public static extern int ca_pend_io(double timeOut_secs_zeroMeansInfinite);

		[DllImport(CA_DLL_NAME)]
		// Tests to see if all ca_get() requests are complete.
		// Returns
		//   ECA_IODONE       - All IO operations completed
		//   ECA_IOINPROGRESS - IO operations still in progress
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_test_io
		public static extern int ca_test_io();

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
		public static extern int ca_pend_event(double timeOut_secs_zeroMeansInfinite);

		[DllImport(CA_DLL_NAME)]
		// Returns the maximum array element count in the server for the specified channel.
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_element_count
		public static extern uint ca_element_count(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// Returns the native 'field type' in the server of the process variable.
		// The returned code will be one of the DBF_ values, or 'DBF_NO_ACCESS'
		// if the channel is disconnected.
		// Could the field type have changed, on a reconnect ? Detect that !!!
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_field_type
		public static extern short ca_field_type(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// Returns boolean true if the client currently has read access
		// to the specified channel and boolean false otherwise.
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_read_access
		public static extern int ca_read_access(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// Returns boolean true if the client currently has write access
		// to the specified channel and boolean false otherwise.
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_write_access
		public static extern int ca_write_access(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_name
		public static extern nint ca_name(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_puser
		public static extern nint ca_puser(nint pChanID);

		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_set_puser
		public static extern void ca_set_puser(nint chan, nint puser);

		[DllImport(CA_DLL_NAME)]
		// https://epics.anl.gov/base/R3-15/9-docs/CAref.html#ca_beacon_period
		public static extern double ca_beacon_period(nint chan);

	}
}
