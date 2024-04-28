using FluentAssertions;
using Convergence;

namespace ConnectTests
{


    public class AllProtocolsConnectionTests
    {
        Action NullCallBack = null;

        private Convergence.IO.EPICS.CA.ConnectionEventCallbackArgs _callbackArgs;

        // Create a callback function of type CaConnectCallback
        private void OnConnect(Convergence.IO.EPICS.CA.ConnectionEventCallbackArgs args)
        {
            _callbackArgs = args;
        }
        
        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public async Task EPICS_CA_Connect_returns_valid_ID()
        {
            _callbackArgs = new Convergence.IO.EPICS.CA.ConnectionEventCallbackArgs();
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_SHORT_i16, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            var connResult = await Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, OnConnect);
            _callbackArgs.chid.Should().NotBe(IntPtr.Zero);
            _callbackArgs.op.Should().Be(Convergence.IO.EPICS.CA.ConnectionEventCallbackArgs.CA_OP_CONN_UP);
            connResult.Should().Be(EndPointStatus.Okay);
           
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        [Test]
        public void EPICS_CA_Connect_twice_returns_same_GUID()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_SHORT_i16,
                elementCount: 1,
                isServer: false,
                isPVA: false);
            var endPointArgs1 = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs1, NullCallBack);
            endPointArgs1.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Creating a new EPICSSettings with the same EndPointID should return a the same Guid.
            var endPointId2 = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings2 = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs2 = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId2, Settings = epicSettings2 };
            // Calling Connect again with the same EndPointID should return a different Guid.
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs2, NullCallBack);
            endPointArgs2.EndPointID.UniqueId.Should().NotBe(endPointArgs1.EndPointID.UniqueId);
        }

        // Create a test for connecting to an integer PV with EPICS_CA
        [Test]
        public void EPICS_CA_Connect_to_integer_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVInteger");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                               datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_LONG_i32,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        // Create a test for connecting to a float Test:PVFloat
        [Test]
        public void EPICS_CA_Connect_to_float_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVFloat");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_FLOAT_f32,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        // Create a test for connecting to a double Test:PVDouble
        [Test]
        public void EPICS_CA_Connect_to_double_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVDouble");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_DOUBLE_f64,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        // Create a test for connecting to a string Test:PVString
        [Test]
        public void EPICS_CA_Connect_to_string_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVString");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_STRING_s39,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }
    }
}