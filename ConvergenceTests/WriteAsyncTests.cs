﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using EPICSCaWriteCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaWriteCallback;
using EPICSCaReadCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaReadCallback;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;
using System.Net.NetworkInformation;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;

namespace WriteTests
{
    public class EPICS_CA_WriteAsyncTests
    {
        [Test]
        public async Task EPICS_CA_WriteAsync_returns_valid_value()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);

            Int16 testValue = 0;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = ConvergenceInstance.Hub.ReadAsync<EPICSCaReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (Int16)epicSettings.DecodeData(value);
                        data.Should().Be(testValue);
                    });
                    readStatus.Result.Should().Be(EndPointStatus.Okay);
                });
                if (status == EndPointStatus.Disconnected)
                {
                    throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVBoolean");
                }
                status.Should().Be(EndPointStatus.Okay);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        // Create a test for writing to an integer PV with EPICS_CA
        [Test]
        public async Task EPICS_CA_WriteAsync_to_integer_PV()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVInteger");
            var epicSettings = new EPICSSettings(
                                    datatype: EPICSDataTypes.DBF_LONG_i32,
                                    elementCount: 1,
                                    isServer: false,
                                    isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);

            Int32 testValue = 655566;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = ConvergenceInstance.Hub.ReadAsync<EPICSCaReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (Int32)epicSettings.DecodeData(value);
                        data.Should().Be(testValue);
                    });
                    readStatus.Result.Should().Be(EndPointStatus.Okay);
                });
                if (status == EndPointStatus.Disconnected)
                {
                    throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVInteger");
                }
                status.Should().Be(EndPointStatus.Okay);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        // Create a test for writing to a float Test:PVFloat
        [Test]
        public async Task EPICS_CA_WriteAsync_to_float_PV()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVFloat");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_FLOAT_f32,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);

            float testValue = 3.14159f;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = ConvergenceInstance.Hub.ReadAsync<EPICSCaReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (float)epicSettings.DecodeData(value);
                        data.Should().Be(testValue);
                    });
                    readStatus.Result.Should().Be(EndPointStatus.Okay);
                });
                if (status == EndPointStatus.Disconnected)
                {
                    throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVFloat");
                }
                status.Should().Be(EndPointStatus.Okay);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        // Create a test for writing to a double Test:PVDouble
        [Test]
        public async Task EPICS_CA_WriteAsync_to_double_PV()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVDouble");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_DOUBLE_f64,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);

            double testValue = double.MaxValue;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = ConvergenceInstance.Hub.ReadAsync<EPICSCaReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (double)epicSettings.DecodeData(value);
                        data.Should().Be(double.MaxValue);
                    });
                    readStatus.Result.Should().Be(EndPointStatus.Okay);
                });
                if (status == EndPointStatus.Disconnected)
                {
                    throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVDouble");
                }
                status.Should().Be(EndPointStatus.Okay);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        // Create a test for writing to a string Test:PVString
        [Test]
        public async Task EPICS_CA_WriteAsync_to_string_PV()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVString");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_STRING_s39,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);

            string testValue = "Hello, World!";
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = Marshal.StringToHGlobalAnsi(testValue);
                // Write async and await a callback
                EndPointStatus status = await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = ConvergenceInstance.Hub.ReadAsync<EPICSCaReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (string)epicSettings.DecodeData(value);
                        data.Should().Be(testValue);
                    });
                    readStatus.Result.Should().Be(EndPointStatus.Okay);
                });
                if (status == EndPointStatus.Disconnected)
                {
                    throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVString");
                }
                status.Should().Be(EndPointStatus.Okay);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }
    }
}
