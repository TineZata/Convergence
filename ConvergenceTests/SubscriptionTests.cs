using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using Convergence.IO.EPICS.CA;

namespace SubscriptionTests
{
    public class EPICS_CA_SubscriptionTests
    {
        Action NullCallBack = null;
        [Test]
        public async Task EPICS_CA_Subscribe_returns_valid_value()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_ENUM_i16,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            Int16 data = -11;
            // Set up a subscription and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.SubscribeAsync<MonitorTypes, Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback>(endPointArgs.EndPointID, MonitorTypes.MonitorValField, (value) =>
            {
                data = (Int16)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVBoolean");
            }
            else {
                GCHandle handle0 = GCHandle.Alloc(0, GCHandleType.Pinned);
                GCHandle handle1 = GCHandle.Alloc(1, GCHandleType.Pinned);
                try
                {
                    // Ensure the PV is set to 0
                    nint valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(0);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    nint valuePtr1 = handle1.AddrOfPinnedObject();
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(1);
                }
                finally
                {
                    if (handle0.IsAllocated)
                        handle0.Free();
                    if (handle1.IsAllocated)
                        handle1.Free();
                }
            }
            status.Should().Be(EndPointStatus.Okay);
            
        }

        // Create a test for subscribing to an integer PV with EPICS_CA
        [Test]
        public async Task EPICS_CA_Subscribe_to_integer_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVInteger");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_LONG_i32,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            Int32 data = -5;
            // Set up a subscription and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.SubscribeAsync<MonitorTypes, Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback>(endPointArgs.EndPointID, MonitorTypes.MonitorValField, (value) =>
            {
                data = (Int32)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVInteger");
            }
            else
            {
                GCHandle handle0 = GCHandle.Alloc(15, GCHandleType.Pinned);
                GCHandle handle1 = GCHandle.Alloc(-5, GCHandleType.Pinned);
                try
                {
                    // Ensure the PV is set to 0
                    nint valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(15);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    nint valuePtr1 = handle1.AddrOfPinnedObject();
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(-5);
                }
                finally
                {
                    if (handle0.IsAllocated)
                        handle0.Free();
                    if (handle1.IsAllocated)
                        handle1.Free();
                }
            }
            status.Should().Be(EndPointStatus.Okay);
        }

        // Create a test for subscribing to a float Test:PVFloat
        [Test]
        public async Task EPICS_CA_Subscribe_to_float_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVFloat");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_FLOAT_f32,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            float data = -5.0f;
            // Set up a subscription and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.SubscribeAsync<MonitorTypes, Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback>(endPointArgs.EndPointID, MonitorTypes.MonitorValField, (value) =>
            {
                data = (float)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVFloat");
            }
            else
            {
                GCHandle handle0 = GCHandle.Alloc(15.0f, GCHandleType.Pinned);
                GCHandle handle1 = GCHandle.Alloc(-5.5f, GCHandleType.Pinned);
                try
                {
                    // Ensure the PV is set to 0
                    nint valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(15.0f);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    nint valuePtr1 = handle1.AddrOfPinnedObject();
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(-5.5f);
                }
                finally
                {
                    if (handle0.IsAllocated)
                        handle0.Free();
                    if (handle1.IsAllocated)
                        handle1.Free();
                }
            }
        }

        // Create a test for subscribing to a double Test:PVDouble
        [Test]
        public async Task EPICS_CA_Subscribe_to_double_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVDouble");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_DOUBLE_f64,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            double data = -5.0;
            // Set up a subscription and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.SubscribeAsync<MonitorTypes, Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback>(endPointArgs.EndPointID, MonitorTypes.MonitorValField, (value) =>
            {
                data = (double)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVDouble");
            }
            else
            {
                GCHandle handle0 = GCHandle.Alloc(double.MaxValue, GCHandleType.Pinned);
                GCHandle handle1 = GCHandle.Alloc(double.MinValue, GCHandleType.Pinned);
                try
                {
                    // Ensure the PV is set to 0
                    nint valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(double.MaxValue);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    nint valuePtr1 = handle1.AddrOfPinnedObject();
                    await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(double.MinValue);
                }
                finally
                {
                    if (handle0.IsAllocated)
                        handle0.Free();
                    if (handle1.IsAllocated)
                        handle1.Free();
                }
            }
        }

        // Create a test for subscribing to a string Test:PVString
        [Test]
        public async Task EPICS_CA_Subscribe_to_string_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVString");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_STRING_s39,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            string data = "Disconnected";
            // Set up a subscription and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.SubscribeAsync<MonitorTypes, Convergence.IO.EPICS.CA.EventCallbackDelegate.MonitorCallback>(endPointArgs.EndPointID, MonitorTypes.MonitorValField, (value) =>
            {
                data = (string)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVString");
            }
            else
            {
                // Ensure the PV is set to 0
                nint valuePtr0 = Marshal.StringToHGlobalAnsi("Ahoy, world");
                // Do a write to the PV to trigger the subscription
                await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                Task.Delay(100).Wait();
                data.Should().Be("Ahoy, world");
                // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                nint valuePtr1 = Marshal.StringToHGlobalAnsi("I'm a string.");
                await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
                Task.Delay(100).Wait();
                data.Should().Be("I'm a string.");
            }
        }
    }
}
