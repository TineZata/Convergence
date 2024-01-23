using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DataTypes;

namespace ConnectTests
{
    public class Tests
    {

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public void Connect_ValidParameters_for_EPICS_CA_ReturnsSession()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, new Guid(), "Test:PV");
            var epicSettings = new EPICSSettings(
                datatype: EPICSDataTypes.CA_DBF_SHORT, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { Id = endPointId, Settings = epicSettings };
            EndPointID result = ConvergenceInstance.Hub.Connect(endPointArgs);
            result.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Connect again with the same EndPointID should return the same Guid.
            EndPointID result2 = ConvergenceInstance.Hub.Connect(endPointArgs);
            result2.UniqueId.Should().Be(result.UniqueId);
        }

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS PV Access, a valid Guid is returned.
        [Test]
        public void Connect_ValidParameters_for_EPICS_PVA_ReturnsSession()
        {
            var endPointId = new EndPointID(Protocols.EPICS_PVA, new Guid(), "Test:PVA");
            var settings = new EPICSSettings(
                datatype: EPICSDataTypes.PVA_int8, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { Settings = settings };
            EndPointID result = ConvergenceInstance.Hub.Connect(endPointArgs);
            result.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Connect again with the same EndPointID should return the same Guid.
            EndPointID result2 = ConvergenceInstance.Hub.Connect(endPointArgs);
            result2.UniqueId.Should().Be(result.UniqueId);
        }
    }
}