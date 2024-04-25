﻿using System;
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
        Action NullCallBack = null;

        [Test]
        public void EPICS_CA_Disconnect_removes_from_hub()
        {
            var endPointId1 = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings1 = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs1 = new EndPointBase<EPICSSettings> { EndPointID = endPointId1, Settings = epicSettings1 };
            ConvergenceInstance.Hub.ConnectAsync(endPointArgs1, NullCallBack);
            endPointArgs1.EndPointID.UniqueId .Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = ConvergenceInstance.Hub.Disconnect(endPointArgs1.EndPointID);
            isDiscon.Should().BeTrue();
            // Creating a new EPICSSettings with the same EndPointID should return a different Guid.
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

        // Create a test for disconnecting from an integer PV with EPICS_CA
        [Test]
        public void EPICS_CA_Disconnect_from_integer_PV()
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
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = ConvergenceInstance.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }
        // Create a test for disconnecting from an Test:PVFloat
        [Test]
        public void EPICS_CA_Disconnect_from_float_PV()
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
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = ConvergenceInstance.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }

        // Create a test for disconnecting from an Test:PVDouble
        [Test]
        public void EPICS_CA_Disconnect_from_double_PV()
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
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = ConvergenceInstance.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }

        // Create a test for disconnecting from an Test:PVString
        [Test]
        public void EPICS_CA_Disconnect_from_string_PV()
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
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = ConvergenceInstance.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }
    }
}
