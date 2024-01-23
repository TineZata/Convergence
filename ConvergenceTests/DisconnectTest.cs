using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DataTypes;

namespace DisconnectTests
{
    public class AllProtocolsDisconnectTests
    {
        [Test]
        public void EPICS_CA_Disconnect_removes_from_hub()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, new Guid(), "Test:PV");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.CA_DBF_SHORT,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            endPointArgs.EndPointID = ConvergenceInstance.Hub.Connect(endPointArgs);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            ConvergenceInstance.Hub.Disconnect(endPointArgs.EndPointID);
            // Calling Connect again with the same EndPointID should return a different Guid.
            EndPointID result2 = ConvergenceInstance.Hub.Connect(endPointArgs);
            result2.UniqueId.Should().NotBe(endPointArgs.EndPointID.UniqueId);
        }
    }
}
