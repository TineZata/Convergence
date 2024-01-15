using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DataTypes;

namespace TestIOCL
{
    public class Tests
    {

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public void Connect_ValidParameters_for_EPICS_CA_ReturnsSession()
        {
            var endPointId = new EndPointID(Protocols.EPICS, new Guid(), "Test:PV");
            var epicSettings = new EPICSSettings(
                datatype: EPICSDataTypes.CA_DBF_SHORT, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { Settings = epicSettings };
            EndPointID result = Convergence.Hub.Connect(endPointId, endPointArgs);
            result.Id.Should().NotBe(Guid.Empty);
        }

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS PV Access, a valid Guid is returned.
        [Test]
        public void Connect_ValidParameters_for_EPICS_PVA_ReturnsSession()
        {
            var endPointId = new EndPointID(Protocols.EPICS, new Guid(), "Test:PVA");
            var settings = new EPICSSettings(
                datatype: EPICSDataTypes.PVA_int8, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<global::Convergence.IO.EPICS.Settings> { Settings = settings };
            EndPointID result = Convergence.Hub.Connect(endPointId, endPointArgs);
            result.Id.Should().NotBe(Guid.Empty);
        }
    }
}