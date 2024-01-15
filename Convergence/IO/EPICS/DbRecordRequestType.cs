//
// DbRecordRequestType.cs
//

namespace Convergence.IO.EPICS;

internal enum DbRecordRequestType : short {

  DBR_STRING        = ChannelAccessConstants.DBR_STRING        ,
  DBR_SHORT         = ChannelAccessConstants.DBR_SHORT         ,
  DBR_FLOAT         = ChannelAccessConstants.DBR_FLOAT         ,
  DBR_ENUM          = ChannelAccessConstants.DBR_ENUM          ,
  DBR_CHAR          = ChannelAccessConstants.DBR_CHAR          ,
  DBR_LONG          = ChannelAccessConstants.DBR_LONG          ,
  DBR_DOUBLE        = ChannelAccessConstants.DBR_DOUBLE        ,

  DBR_STS_          = ChannelAccessConstants.DBR_STS_STRING    ,
  DBR_STS_STRING    = ChannelAccessConstants.DBR_STS_STRING    ,
  DBR_STS_SHORT     = ChannelAccessConstants.DBR_STS_SHORT     ,
  DBR_STS_FLOAT     = ChannelAccessConstants.DBR_STS_FLOAT     ,
  DBR_STS_ENUM      = ChannelAccessConstants.DBR_STS_ENUM      ,
  DBR_STS_CHAR      = ChannelAccessConstants.DBR_STS_CHAR      ,
  DBR_STS_LONG      = ChannelAccessConstants.DBR_STS_LONG      ,
  DBR_STS_DOUBLE    = ChannelAccessConstants.DBR_STS_DOUBLE    ,

  DBR_TIME_         = ChannelAccessConstants.DBR_TIME_STRING   ,
  DBR_TIME_STRING   = ChannelAccessConstants.DBR_TIME_STRING   ,
  DBR_TIME_SHORT    = ChannelAccessConstants.DBR_TIME_SHORT    ,
  DBR_TIME_FLOAT    = ChannelAccessConstants.DBR_TIME_FLOAT    ,
  DBR_TIME_ENUM     = ChannelAccessConstants.DBR_TIME_ENUM     ,
  DBR_TIME_CHAR     = ChannelAccessConstants.DBR_TIME_CHAR     ,
  DBR_TIME_LONG     = ChannelAccessConstants.DBR_TIME_LONG     ,
  DBR_TIME_DOUBLE   = ChannelAccessConstants.DBR_TIME_DOUBLE   ,

  DBR_GR_           = ChannelAccessConstants.DBR_GR_STRING     ,
  DBR_GR_STRING     = ChannelAccessConstants.DBR_GR_STRING     ,
  DBR_GR_SHORT      = ChannelAccessConstants.DBR_GR_SHORT      ,
  DBR_GR_FLOAT      = ChannelAccessConstants.DBR_GR_FLOAT      ,
  DBR_GR_ENUM       = ChannelAccessConstants.DBR_GR_ENUM       ,
  DBR_GR_CHAR       = ChannelAccessConstants.DBR_GR_CHAR       ,
  DBR_GR_LONG       = ChannelAccessConstants.DBR_GR_LONG       ,
  DBR_GR_DOUBLE     = ChannelAccessConstants.DBR_GR_DOUBLE     ,

  DBR_CTRL_         = ChannelAccessConstants.DBR_CTRL_STRING   ,
  DBR_CTRL_STRING   = ChannelAccessConstants.DBR_CTRL_STRING   ,
  DBR_CTRL_SHORT    = ChannelAccessConstants.DBR_CTRL_SHORT    ,
  DBR_CTRL_FLOAT    = ChannelAccessConstants.DBR_CTRL_FLOAT    ,
  DBR_CTRL_ENUM     = ChannelAccessConstants.DBR_CTRL_ENUM     ,
  DBR_CTRL_CHAR     = ChannelAccessConstants.DBR_CTRL_CHAR     ,
  DBR_CTRL_LONG     = ChannelAccessConstants.DBR_CTRL_LONG     ,
  DBR_CTRL_DOUBLE   = ChannelAccessConstants.DBR_CTRL_DOUBLE   ,

  DBR_PUT_ACKT      = ChannelAccessConstants.DBR_PUT_ACKT      ,
  DBR_PUT_ACKS      = ChannelAccessConstants.DBR_PUT_ACKS      ,
  DBR_STSACK_STRING = ChannelAccessConstants.DBR_STSACK_STRING ,

  DBR_CLASS_NAME    = ChannelAccessConstants.DBR_CLASS_NAME    ,

}
