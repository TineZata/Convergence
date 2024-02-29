using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;
using System.Net.NetworkInformation;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;

namespace ReadTests
{
    public class EPICS_CA_ReadAsyncTests
    {
        [Test]
        public async Task EPICS_CA_ReadAsync_returns_valid_value()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            ConvergenceInstance.Hub.Connect(endPointArgs);
            
            Int16 data = -1;
            // Read async and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.ReadAsync(endPointArgs.EndPointID, (value) =>
            {
                data = (Int16)epicSettings.DecodeData(value);
            });
            Task.Delay(3000).Wait();
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be(1);
        }
    }
}
