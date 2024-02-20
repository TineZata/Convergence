using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DataTypes;
using System.Net.NetworkInformation;

namespace ReadTests
{
    public class EPICS_CA_ReadAsyncTests
    {
        [Test]
        public void EPICS_CA_ReadAsync_returns_valid_value()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.CA_DBF_SHORT,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.Connect(endPointArgs);

            bool readAsyncCalled = false;
            // Read async and await a callback
            Task<EndPointStatus> task1 = ConvergenceInstance.Hub.ReadAsync(endPointArgs.EndPointID, (value) =>
            {
                value.Should().BeOfType<short>();
                value.Should().Be(1);
                readAsyncCalled = true;
            });
            task1.Result.Should().Be(EndPointStatus.Okay);
            readAsyncCalled.Should().BeTrue();
        }
    }
}
