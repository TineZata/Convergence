using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using EPICSCaWriteCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaWriteCallback;
using EPICSCaReadCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaReadCallback;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;
using System.Net.NetworkInformation;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;

namespace WriteTests
{
    public class EPICS_CA_WriteAsyncTests
    {
        [Test]
        public async Task EPICS_CA_WriteAsync_returns_valid_value()
        {
            // Create a new connections and then attempt to write the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);

            Int16 testValue = 0;
            GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
            try
            {
                IntPtr valuePtr = handle.AddrOfPinnedObject();
                // Write async and await a callback
                EndPointStatus status = await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, (_) =>
                {
                    // Read the value back to verify the write
                    var readStatus = ConvergenceInstance.Hub.ReadAsync<EPICSCaReadCallback>(endPointArgs.EndPointID, (value) =>
                    {
                        var data = (Int16)epicSettings.DecodeData(value);
                        data.Should().Be(testValue);
                    });
                    readStatus.Result.Should().Be(EndPointStatus.Okay);
                });
                if (status == EndPointStatus.Disconnected)
                {
                    throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PV");
                }
                status.Should().Be(EndPointStatus.Okay);
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }
    }
}
