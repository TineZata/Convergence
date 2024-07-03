namespace Convergence.IO.EPICS.CA
{
    [Flags]
    public enum MonitorTypes
    {
        MonitorValField = ChannelAccessConstants.DBE_VALUE,
        MonitorOtherFields = ChannelAccessConstants.DBE_PROPERTY,
        MonitorAlarmFields = ChannelAccessConstants.DBE_ALARM,
        MonitorLogFields = ChannelAccessConstants.DBE_LOG,
        MonitorAllFields =
          MonitorValField
        | MonitorOtherFields
        | MonitorAlarmFields
        | MonitorLogFields

    }
}
