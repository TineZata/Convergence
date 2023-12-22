using FluentAssertions;
using ConvergenceLib;

namespace TestIOCL
{
    public class Tests
    {

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS Channel Access, a valid Guid is returned.
        [Test]
        public void Connect_ValidParameters_for_EPICS_CA_ReturnsSession()
        {
            var settings = new ConvergenceLib.IO.EPICS.Settings(isPVA: false);
            var args = new EndPointBase<ConvergenceLib.IO.EPICS.Settings> { Settings = settings };
            var result = Convergence.Hub.Connect(Protocols.EPICS, "Test:PV", args);
            result.Should().NotBe(Guid.Empty);
        }

        // Test for ConvergenceLib.IO.EPICS.Settings is set correctlty for EPICS PV Access, a valid Guid is returned.
        public void Connect_ValidParameters_for_EPICS_PVA_ReturnsSession()
        {
            var settings = new ConvergenceLib.IO.EPICS.Settings(isPVA: true);
            var args = new EndPointBase<ConvergenceLib.IO.EPICS.Settings> { Settings = settings };
            var result = Convergence.Hub.Connect(Protocols.EPICS, "Test:PVA", args);
            result.Should().NotBe(Guid.Empty);
        }
    }
}