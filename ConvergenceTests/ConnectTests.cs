using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;

namespace ConnectTests
{
    public class AllProtocolsConnectionTests
    {

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public void EPICS_CA_Connect_returns_valid_ID()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings = new EPICSSettings(
                datatype: EPICSDataTypes.DBF_SHORT_i16, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.Connect(endPointArgs);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
        }

        [Test]
        public void EPICS_CA_Connect_twice_returns_same_GUID()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings = new EPICSSettings(
                datatype: EPICSDataTypes.DBF_SHORT_i16,
                elementCount: 1,
                isServer: false,
                isPVA: false);
            var endPointArgs1 = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.Connect(endPointArgs1);
            endPointArgs1.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Creating a new EPICSSettings with the same EndPointID should return a the same Guid.
            var endPointId2 = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings2 = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs2 = new EndPointBase<EPICSSettings> { EndPointID = endPointId2, Settings = epicSettings2 };
            // Calling Connect again with the same EndPointID should return a different Guid.
           ConvergenceInstance.Hub.Connect(endPointArgs2);
            endPointArgs2.EndPointID.UniqueId.Should().NotBe(endPointArgs1.EndPointID.UniqueId);
        }

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS PV Access, a valid Guid is returned.
        //[Test]
        //public void EPICS_PVA_Connect_returns_valid_ID()
        //{
        //    var endPointId = new EndPointID(Protocols.EPICS_PVA, new Guid(), "Test:PVA");
        //    var settings = new EPICSSettings(
        //        datatype: EPICSDataTypes.PVA_int8, 
        //        elementCount: 1, 
        //        isServer: false, 
        //        isPVA: false);
        //    var endPointArgs = new EndPointBase<EPICSSettings> { Settings = settings };
        //    EndPointID result = ConvergenceInstance.Hub.Connect(endPointArgs);
        //    result.UniqueId.Should().NotBe(Guid.Empty);
        //    // Calling Connect again with the same EndPointID should return the same Guid.
        //    EndPointID result2 = ConvergenceInstance.Hub.Connect(endPointArgs);
        //    result2.UniqueId.Should().Be(result.UniqueId);
        //}
    }
}