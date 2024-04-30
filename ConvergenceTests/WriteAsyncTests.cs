using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace WriteTests
{
    public class EPICS_CA_WriteAsyncTests
    {
        Action NullCallBack = null;
        [Test]
        public async Task EPICS_CA_WriteAsync_returns_valid_value()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_ENUM_i16,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);

            Int16 testValue = 0;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (Int16)epicSettings.DecodeEventData(value);
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
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_LONG_i32,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);

            Int32 testValue = 655566;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (Int32)epicSettings.DecodeEventData(value);
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
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_FLOAT_f32,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);

            float testValue = 3.14159f;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (float)epicSettings.DecodeEventData(value);
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
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_DOUBLE_f64,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);

            double testValue = double.MaxValue;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (double)epicSettings.DecodeEventData(value);
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
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_STRING_s39,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);

            string testValue = "Hello, World!";
            IntPtr valuePtr = Marshal.StringToHGlobalAnsi(testValue);
            // Write async and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.WriteAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.WriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
            {
                // Read the value back to verify the write
                var readStatus = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
                {
                    var data = (string)epicSettings.DecodeEventData(value);
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
    }
}
