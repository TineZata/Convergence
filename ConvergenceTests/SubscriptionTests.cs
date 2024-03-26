﻿using System;
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
                data = (Int16)epicSettings.DecodeData(value);
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
                data = (Int32)epicSettings.DecodeData(value);
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
    }
}
