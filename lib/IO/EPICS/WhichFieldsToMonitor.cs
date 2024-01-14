using Convergence.IO.EPICS;

namespace Conversion.IO.EPICS
{

  [System.Flags]
  public enum WhichFieldsToMonitor {
    MonitorValField    = ChannelAccessConstants.DBE_VALUE,   
    MonitorOtherFields = ChannelAccessConstants.DBE_PROPERTY,
    MonitorAlarmFields = ChannelAccessConstants.DBE_ALARM,   
    MonitorLogFields   = ChannelAccessConstants.DBE_LOG,     
    MonitorAllFields   = (
      MonitorValField 
    | MonitorOtherFields 
    | MonitorAlarmFields 
    | MonitorLogFields 
    )
  }

}
