using System;
using System.Collections.Generic;
using FluentAssertions;
using Convergence;
using EPICSSettings = Convergence.IO.EPICS.Settings;
using EPICSDataTypes = Convergence.IO.EPICS.DbFieldType;
using EPICSCaMonitorCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaMonitorCallback;
using EPICSCaWriteCallback = Convergence.IO.EPICS.CaEventCallbackDelegate.CaWriteCallback;
using System.Net.NetworkInformation;
using Convergence.IO.EPICS;
using System.Runtime.InteropServices;
using Conversion.IO.EPICS;

namespace SubscriptionTests
{
    public class EPICS_CA_SubscriptionTests
    {
        [Test]
        public async Task EPICS_CA_Subscribe_returns_valid_value()
        {
            // Create a new connections and then attempt to read the value.
            var endPointId = new EndPointID(Protocols.EPICS_CA, "Test:PV");
            var epicSettings = new EPICSSettings(
                                datatype: EPICSDataTypes.DBF_SHORT_i16,
                                elementCount: 1,
                                isServer: false,
                                isPVA: false);
            var endPointArgs = new EndPointBase<EPICSSettings> { EndPointID = endPointId, Settings = epicSettings };
            await ConvergenceInstance.Hub.ConnectAsync(endPointArgs);
            
            Int16 data = -11;
            // Set up a subscription and await a callback
            EndPointStatus status = await ConvergenceInstance.Hub.SubscribeAsync<CaMonitorTypes, EPICSCaMonitorCallback>(endPointArgs.EndPointID, CaMonitorTypes.MonitorValField, (value) =>
            {
                data = (Int16)epicSettings.DecodeData(value);
            });
            if (status == EndPointStatus.Disconnected)
            {
                throw new Exception("Disconnected: Make sure you are running an IOC with pvname = Test:PV");
            }
            else {
                Int16 testValue = 1;
                GCHandle handle = GCHandle.Alloc(testValue, GCHandleType.Pinned);
                try
                {
                    IntPtr valuePtr = handle.AddrOfPinnedObject();
                    // Do a write to the PV to trigger the subscription
                    await ConvergenceInstance.Hub.WriteAsync<EPICSCaWriteCallback>(endPointArgs.EndPointID, valuePtr, null);
                }
                finally
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
            }
            status.Should().Be(EndPointStatus.Okay);
            data.Should().Be(1);
        }
    }
}
