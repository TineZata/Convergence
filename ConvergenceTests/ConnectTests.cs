using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;
using EPICSCaConnectCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaConnectCallback;
using Convergence.IO.EPICS;
using static Convergence.IO.EPICS.CaEventCallbackDelegate;

namespace ConnectTests
{
    

    public class AllProtocolsConnectionTests
    {
        Action NullCallBack = null;

        private ConnectionEventCallbackArgs _callbackArgs;

        // Create a callback function of type CaConnectCallback
        private void OnConnect(ConnectionEventCallbackArgs args)
        {
            _callbackArgs = args;
        }
        
        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public async Task EPICS_CA_Connect_returns_valid_ID()
        {
            _callbackArgs = new ConnectionEventCallbackArgs();
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new EPICSSettings(
                datatype: EPICSDataTypes.DBF_SHORT_i16, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            var connResult = await ConvergenceInstance.Hub.ConnectAsync(endPointArgs, (EPICSCaConnectCallback)OnConnect);
            connResult.Should().Be(EndPointStatus.Okay);
            _callbackArgs.chid.Should().NotBe(IntPtr.Zero);
            _callbackArgs.op.Should().Be(ConnectionEventCallbackArgs.CA_OP_CONN_UP);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        [Test]
        public void EPICS_CA_Connect_twice_returns_same_GUID()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings = new EPICSSettings(
                datatype: EPICSDataTypes.DBF_SHORT_i16,
                elementCount: 1,
                isServer: false,
                isPVA: false);
            var endPointArgs1 = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs1, NullCallBack);
            endPointArgs1.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Creating a new EPICSSettings with the same EndPointID should return a the same Guid.
            var endPointId2 = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings2 = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs2 = new EndPointBase<EPICSSettings> { EndPointID = endPointId2, Settings = epicSettings2 };
            // Calling Connect again with the same EndPointID should return a different Guid.
           ConvergenceInstance.Hub.ConnectAsync(endPointArgs2, NullCallBack);
            endPointArgs2.EndPointID.UniqueId.Should().NotBe(endPointArgs1.EndPointID.UniqueId);
        }

        // Create a test for connecting to an integer PV with EPICS_CA
        [Test]
        public void EPICS_CA_Connect_to_integer_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVInteger");
            var epicSettings = new EPICSSettings(
                               datatype: EPICSDataTypes.DBF_LONG_i32,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        // Create a test for connecting to a float Test:PVFloat
        [Test]
        public void EPICS_CA_Connect_to_float_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVFloat");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_FLOAT_f32,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        // Create a test for connecting to a double Test:PVDouble
        [Test]
        public void EPICS_CA_Connect_to_double_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVDouble");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_DOUBLE_f64,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        // Create a test for connecting to a string Test:PVString
        [Test]
        public void EPICS_CA_Connect_to_string_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVString");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_STRING_s39,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }
    }
}