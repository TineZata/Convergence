using FluentAssertions;
using Convergence;

namespace TestIOCL
{
    public class Tests
    {

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public void Connect_ValidParameters_for_EPICS_CA_ReturnsSession()
        {
            var endPointId = new EndPointID(Protocols.EPICS, new Guid(), "Test:PV");
            var settings = new global::Convergence.IO.EPICS.Settings(
                datatype: global::Convergence.IO.EPICS.DataTypes.CA_DBF_SHORT, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<global::Convergence.IO.EPICS.Settings> { Settings = settings };
            EndPointID result = Convergence.Hub.Connect(endPointId, endPointArgs);
            result.Should().NotBe(Guid.Empty);
        }

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS PV Access, a valid Guid is returned.
        public void Connect_ValidParameters_for_EPICS_PVA_ReturnsSession()
        {
            var endPointId = new EndPointID(Protocols.EPICS, new Guid(), "Test:PVA");
            var settings = new global::Convergence.IO.EPICS.Settings(
                datatype: global::Convergence.IO.EPICS.DataTypes.PVA_int8, 
                elementCount: 1, 
                isServer: false, 
                isPVA: false);
            var endPointArgs = new EndPointBase<global::Convergence.IO.EPICS.Settings> { Settings = settings };
            EndPointID result = Convergence.Hub.Connect(endPointId, endPointArgs);
            result.Should().NotBe(Guid.Empty);
        }
    }
}