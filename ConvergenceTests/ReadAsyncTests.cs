using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using Convergence.IO.EPICS.CA;

namespace ReadTests
{
    public class EPICS_CA_ReadAsyncTests
    {
        Action NullCallBack = null;
        [Test]
        public async Task EPICS_CA_ReadAsync_returns_boolean_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_ENUM_i16,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            Int16 data = -1;
            // Read async and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
            {
                data = (Int16)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVBoolean");
            }
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be(1);
        }

        // Create a test for reading from an integer PV with EPICS_CA
        [Test]
        public async Task EPICS_CA_ReadAsync_from_integer_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVInteger");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_LONG_i32,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            Int32 data = -5;
            // Read async and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
            {
                data = (Int32)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVInteger");
            }
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be(-5);
        }

        // Create a test for reading from a float Test:PVFloat
        [Test]
        public async Task EPICS_CA_ReadAsync_from_float_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVFloat");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_FLOAT_f32,
                                elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            float data = -6.1f;
            // Read async and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
            {
                data = (float)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVFloat");
            }
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be(-5.5f);
        }

        // Create a test for reading from a double Test:PVDouble
        [Test]
        public async Task EPICS_CA_ReadAsync_from_double_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVDouble");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_DOUBLE_f64,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            double data = -6.1;
            // Read async and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
            {
                data = (double)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVDouble");
            }
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be(double.MinValue);
        }

        // Create a test for reading from a string Test:PVString
        [Test]
        public async Task EPICS_CA_ReadAsync_from_string_PV()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVString");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_STRING_s39,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            
            string data = "Disconnected";
            // Read async and await a callback
            EndPointStatus status = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ReadAsync<Convergence.IO.EPICS.CA.EventCallbackDelegate.ReadCallback>(endPointArgs.EndPointID, (value) =>
            {
                data = (string)Helpers.DecodeEventData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PVString");
            }
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be("I'm a string.");
        }
    }
}
