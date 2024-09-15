
using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using static Convergence.IO.EPICS.CA.EventCallbackDelegate;

namespace Convergence.IO.EPICS.CA
{

    internal static class ChannelAccessWrapper
    {
        static ChannelAccessWrapper()
        {
        }

        public static nint CurrentContext { get; private set; }

        private static void set_current_context()
        {
            try
            {
                var context = CANativeMethods.ca_current_context();
                // Check if CurrentContext has been created
                if (CurrentContext == nint.Zero)
                {
					CANativeMethods.ca_attach_context(context); 
					// Store a record of the current context
					CurrentContext = context;
                }
                // else if CurrentContext is not nint.Zero but not equal to context
    //            else if (CurrentContext != context)
    //            {
				//	// Destroy the CurrentContext
				//	CANativeMethods.ca_context_destroy();
    //                Console.WriteLine("! TZ: CurrentContext has been destroyed");
				//	// Attach to a new context
				//	CANativeMethods.ca_attach_context(context);
				//	CurrentContext = context;
				//}
                else
                {
                    // Do nothing
                }
			}
			catch (TaskCanceledException  ex)
			{
				throw new Exception("Error setting current context", ex);
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
            if (Enum.TryParse<EcaType>(CA_EXTRACT_MSG_NO(CANativeMethods.ca_context_create(preemptive)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_context_create: Unable to cast EcaType from Int32");
            }
        }

        // Needed when you invoke a 'ca_' function on a worker thread

        public static nint ca_current_context()
        {
            return CANativeMethods.ca_current_context();
            
        }

        public static EcaType ca_attach_context(this nint context)
        {
            // Try Parse Enum of type <EcaType> from Int32 return by (ca_attach_context(context))
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_attach_context(context)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_attach_context: Unable to cast EcaType from Int32");
            }
            
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
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_create_channel(
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
		}

        public static EcaType ca_clear_channel(nint pChanID)
        {
            try
            {
                if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_clear_channel(pChanID)).ToString(), out EcaType result))
                {
                    return result;
                }
                else
                {
                    throw new InvalidCastException("ca_clear_channel: Unable to cast EcaType from Int32");
                }

                
            } catch (TaskCanceledException  ex) {
				throw new Exception("Error clearing channel", ex);
			}
            catch (AccessViolationException ex)
            {
				throw new Exception("Error clearing channel", ex);
			}

		}

        // -------------
        // CHANNEL STATE
        // -------------

        public static ChannelState ca_state(nint pChanID)
        {
            return CANativeMethods.ca_state(pChanID);
        }

        public static string ca_host_name(nint pChanID)
        {
            return Marshal.PtrToStringAnsi(
              CANativeMethods.ca_host_name(pChanID)
            )!;
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
            try
            {

                if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_array_get(
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
                
			}
			catch (TaskCanceledException  ex)
			{
				throw new Exception("Error getting array", ex);
			}
		}

        public static EcaType ca_array_get_callback(
          this nint pChanID,
          DbFieldType type,
          int nElements,
          ReadCallback valueUpdateCallBack
        )
        {
            try
            {
                // Check that the channel is connected
                if (CANativeMethods.ca_state(pChanID) != ChannelState.CurrentlyConnected) return EcaType.ECA_DISCONN;
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
                    CANativeMethods.ca_array_get_callback(
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
                
            }
			catch (TaskCanceledException  ex)
			{
				throw new Exception("Error getting array with callback", ex);
			}

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
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_array_put(
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
            if (CANativeMethods.ca_state(pChanID) != ChannelState.CurrentlyConnected) return EcaType.ECA_DISCONN;
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

            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_array_put_callback(
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
            if (CANativeMethods.ca_state(pChanID) != ChannelState.CurrentlyConnected) return EcaType.ECA_DISCONN;
            // Check that read access is allowed
            if (!pChanID.ca_read_access()) return EcaType.ECA_NORDACCESS;
            // assign userArg to ca_puser(pChanID)
            var userArg = pChanID.ca_puser();
            whichFieldsToMonitor ??= MonitorTypes.MonitorValField;
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_create_subscription(
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
            
        }

        public static EcaType ca_clear_subscription(this nint pChanID)
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_clear_subscription(pChanID)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_clear_subscription: Unable to cast EcaType from Int32");
            }
            
        }

        // ------------
        // FLUSH_IO etc
        // ------------

        public static EcaType ca_flush_io()
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_flush_io()).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_flush_io: Unable to cast EcaType from Int32");
            }
            
        }


        public static EcaType ca_pend_io(double timeOut_secs_zeroMeansInfinite)
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_pend_io(timeOut_secs_zeroMeansInfinite)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_pend_io: Unable to cast EcaType from Int32");
            }

            
        }

        public static EcaType ca_test_io()
        {
            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_test_io()).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_test_io: Unable to cast EcaType from Int32");
            }
            
        }

        public static EcaType ca_pend_event(double nSecsToBlock_zeroMeansInfinite)
        {

            if (Enum.TryParse(CA_EXTRACT_MSG_NO(CANativeMethods.ca_pend_event(nSecsToBlock_zeroMeansInfinite)).ToString(), out EcaType result))
            {
                return result;
            }
            else
            {
                throw new InvalidCastException("ca_pend_event: Unable to cast EcaType from Int32");
            }
            
        }

        // -------
        // QUERIES
        // -------

        // These query functions don't require a message to the server,
        // once a connection has been establised initially.

        // This tells us the *max* number of elements that the server will deal with.

        public static uint ca_element_count(this nint pChanID)
        {
            return CANativeMethods.ca_element_count(pChanID);
            
        }

        public static short ca_field_type(this nint pChanID)
        {
            return CANativeMethods.ca_field_type(pChanID);

            
        }

        // If we have write access, do we always have 'read' access ??
        // Might have a PV where writing anything triggers an action,
        // but the value itself isn't readable ?
        // In that case, we probably need a special FieldType ???

        public static bool ca_read_access(this nint pChanID)
        {
            return CANativeMethods.ca_read_access(pChanID) != 0;
            
        }

        public static bool ca_write_access(this nint pChanID)
        {
            return CANativeMethods.ca_write_access(pChanID) != 0;
            
        }

        public static string ca_name(this nint pChanID)
        {
            return Marshal.PtrToStringAnsi(
			  CANativeMethods.ca_name(pChanID)
            )!;
            
        }

        // Get the 'user info tag' associated with the channel

        public static nint ca_puser(this nint pChanID)
        {
            return (int)CANativeMethods.ca_puser(pChanID);
            
        }

        // Set the 'user info tag' associated with the channel

        public static void ca_set_puser(this nint pChanID, nint userInfo)
        {
            CANativeMethods.ca_set_puser(pChanID, userInfo);
            
        }

        // Get the 'beacon period' for the channel.
        // Hmm, this might be useful to know, however the value comes back as a junk value
        // even if we wait until the channel has been successfully connected-to ...

        public static double ca_beacon_period(this nint pChanID)
        {
            return CANativeMethods.ca_beacon_period(pChanID);
        }

        private static int CA_EXTRACT_MSG_NO(int eca_code) => (eca_code & ChannelAccessConstants.CA_M_MSG_NO) >> ChannelAccessConstants.CA_V_MSG_NO;
    }

}
