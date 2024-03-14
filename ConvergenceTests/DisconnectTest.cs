using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;

namespace DisconnectTests
{
    public class AllProtocolsDisconnectTests
    {
        [Test]
        public void EPICS_CA_Disconnect_removes_from_hub()
        {
            var endPointId1 = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings1 = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs1 = new EndPointBase<EPICSSettings> { EndPointID = endPointId1, Settings = epicSettings1 };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs1);
            endPointArgs1.EndPointID.UniqueId .Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            ConvergenceInstance.Hub.Disconnect(endPointArgs1.EndPointID);
            // Creating a new EPICSSettings with the same EndPointID should return a different Guid.
            var endPointId2 = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings2 = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs2 = new EndPointBase<EPICSSettings> { EndPointID = endPointId2, Settings = epicSettings2 };
            // Calling Connect again with the same EndPointID should return a different Guid.
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs2);
            endPointArgs2.EndPointID.UniqueId.Should().NotBe(endPointArgs1.EndPointID.UniqueId);
        }
    }
}
