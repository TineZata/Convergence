using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;
using EPICSCaMonitorCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaMonitorCallback;
using EPICSCaWriteCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaWriteCallback;
using System.Net.NetworkInformation;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;
using Conversion.IO.EPICS;

namespace SubscriptionTests
{
    public class EPICS_CA_SubscriptionTests
    {
        [Test]
        public async Task EPICS_CA_Subscribe_returns_valid_value()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);
            
            Int16 data = -11;
            // Set up a subscription and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.SubscribeAsync<CaMonitorTypes, EPICSCaMonitorCallback>(endPointArgs.EndPointID, CaMonitorTypes.MonitorValField, (value) =>
            {
                data = (Int16)epicSettings.DecodeEventData(value);
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
                    IntPtr valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(0);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    IntPtr valuePtr1 = handle1.AddrOfPinnedObject();
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
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
            var epicSettings = new EPICSSettings(
                                    datatype: EPICSDataTypes.DBF_LONG_i32,
                                    elementCount: 1,
                                    isServer: false,
                                    isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);
            
            Int32 data = -5;
            // Set up a subscription and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.SubscribeAsync<CaMonitorTypes, EPICSCaMonitorCallback>(endPointArgs.EndPointID, CaMonitorTypes.MonitorValField, (value) =>
            {
                data = (Int32)epicSettings.DecodeEventData(value);
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
                    IntPtr valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(15);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    IntPtr valuePtr1 = handle1.AddrOfPinnedObject();
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
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
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_FLOAT_f32,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);
            
            float data = -5.0f;
            // Set up a subscription and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.SubscribeAsync<CaMonitorTypes, EPICSCaMonitorCallback>(endPointArgs.EndPointID, CaMonitorTypes.MonitorValField, (value) =>
            {
                data = (float)epicSettings.DecodeEventData(value);
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
                    IntPtr valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(15.0f);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    IntPtr valuePtr1 = handle1.AddrOfPinnedObject();
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
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
            var epicSettings = new EPICSSettings(
                                    datatype: EPICSDataTypes.DBF_DOUBLE_f64,
                                    elementCount: 1,
                                    isServer: false,
                                    isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);
            
            double data = -5.0;
            // Set up a subscription and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.SubscribeAsync<CaMonitorTypes, EPICSCaMonitorCallback>(endPointArgs.EndPointID, CaMonitorTypes.MonitorValField, (value) =>
            {
                data = (double)epicSettings.DecodeEventData(value);
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
                    IntPtr valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be(double.MaxValue);
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    IntPtr valuePtr1 = handle1.AddrOfPinnedObject();
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
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
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_STRING_s39,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);
            
            string data = "Disconnected";
            // Set up a subscription and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.SubscribeAsync<CaMonitorTypes, EPICSCaMonitorCallback>(endPointArgs.EndPointID, CaMonitorTypes.MonitorValField, (value) =>
            {
                data = (string)epicSettings.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVString");
            }
            else
            {
                GCHandle handle0 = GCHandle.Alloc("Ahoy, world", GCHandleType.Pinned);
                GCHandle handle1 = GCHandle.Alloc("I'm a string.", GCHandleType.Pinned);
                try
                {
                    // Ensure the PV is set to 0
                    IntPtr valuePtr0 = handle0.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr0, null);
                    Task.Delay(100).Wait();
                    data.Should().Be("Ahoy, world");
                    // Now write 1 to the PV... this way we ensure that the subscription is called at least once.
                    IntPtr valuePtr1 = handle1.AddrOfPinnedObject();
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr1, null);
                    Task.Delay(100).Wait();
                    data.Should().Be("I'm a string.");
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
    }
}
