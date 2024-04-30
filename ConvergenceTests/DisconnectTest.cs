using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Convergence;

namespace DisconnectTests
{
    public class AllProtocolsDisconnectTests
    {
        Action NullCallBack = null;

        [Test]
        public void EPICS_CA_Disconnect_removes_from_hub()
        {
            var endPointId1 = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings1 = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_SHORT_i16,
                                elementCount: 1);
            var endPointArgs1 = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId1, Settings = epicSettings1 };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs1, NullCallBack);
            endPointArgs1.EndPointID.UniqueId .Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endPointArgs1.EndPointID);
            isDiscon.Should().BeTrue();
            // Creating a new Convergence.IO.EPICS.CA.Settings with the same EndPointID should return a different Guid.
            var endPointId2 = new EndPointID(Protocols.EPICS_CA, "Test:PVBoolean");
            var epicSettings2 = new Convergence.IO.EPICS.CA.Settings(
                                datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_SHORT_i16,
                                elementCount: 1);
            var endPointArgs2 = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId2, Settings = epicSettings2 };
            // Calling Connect again with the same EndPointID should return a different Guid.
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs2, NullCallBack);
            endPointArgs2.EndPointID.UniqueId.Should().NotBe(endPointArgs1.EndPointID.UniqueId);
        }

        // Create a test for disconnecting from an integer PV with EPICS_CA
        [Test]
        public void EPICS_CA_Disconnect_from_integer_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVInteger");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_LONG_i32,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }
        // Create a test for disconnecting from an Test:PVFloat
        [Test]
        public void EPICS_CA_Disconnect_from_float_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVFloat");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_FLOAT_f32,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }

        // Create a test for disconnecting from an Test:PVDouble
        [Test]
        public void EPICS_CA_Disconnect_from_double_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVDouble");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_DOUBLE_f64,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }

        // Create a test for disconnecting from an Test:PVString
        [Test]
        public void EPICS_CA_Disconnect_from_string_PV()
        {
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PVString");
            var epicSettings = new Convergence.IO.EPICS.CA.Settings(
                                    datatype: Convergence.IO.EPICS.CA.DbFieldType.DBF_STRING_s39,
                                    elementCount: 1);
            var endPointArgs = new EndPointBase<Convergence.IO.EPICS.CA.Settings> { EndPointID = endPointId, Settings = epicSettings };
            Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.ConnectAsync(endPointArgs, NullCallBack);
            endPointArgs.EndPointID.UniqueId.Should().NotBe(Guid.Empty);
            // Calling Disconnect should remove the EndPointID from the Hub.
            var isDiscon = Convergence.IO.EPICS.CA.ConvergeOnEPICSChannelAccess.Hub.Disconnect(endPointArgs.EndPointID);
            isDiscon.Should().BeTrue();
        }
    }
}
