namespace Convergence.IO.EPICS
{
    public enum EcaType
    {
        /*
         * In the lines below "defunct" indicates that current release 
         * servers and client library will not return this error code, but
         * servers on earlier releases that communicate with current clients 
         * might still generate exceptions with these error constants
         */
        ECA_NORMAL = 0,       // success

        ECA_MAXIOC = 1,      // defunct
        ECA_UKNHOST = 2,     // defunct
        ECA_UKNSERV = 3,     // defunct

        ECA_SOCK = 4,        // defunct
        ECA_CONN = 5,        // defunct

        ECA_ALLOCMEM = 6,

        ECA_UKNCHAN = 7,     // defunct
        ECA_UKNFIELD = 8,    // defunct

        ECA_TOLARGE = 9,
        ECA_TIMEOUT = 10,

        ECA_NOSUPPORT = 11,   // defunct
        ECA_STRTOBIG = 12,   // defunct

        ECA_DISCONNCHID = 13, // defunct

        ECA_BADTYPE = 14,

        ECA_CHIDNOTFND = 15, // defunct
        ECA_CHIDRETRY = 16,   // defunct

        ECA_INTERNAL = 17,

        ECA_DBLCLFAIL = 18, // defunct

        ECA_GETFAIL = 19,
        ECA_PUTFAIL = 20,

        ECA_ADDFAIL = 21, // defunct

        ECA_BADCOUNT = 22,
        ECA_BADSTR = 23,

        ECA_DISCONN = 24,
        ECA_DBLCHNL = 25,

        ECA_EVDISALLOW = 26,

        ECA_BUILDGET = 27, // defunct
        ECA_NEEDSFP = 28, // defunct
        ECA_OVEVFAIL = 29, // defunct

        ECA_BADMONID = 30,

        ECA_NEWADDR = 31, // defunct
        ECA_NEWCONN = 32, // defunct

        ECA_NOCACTX = 33, // defunct
        ECA_DEFUNCT = 34, // defunct

        ECA_EMPTYSTR = 35, // defunct
        ECA_NOREPEATER = 36, // defunct

        ECA_NOCHANMSG = 37, // defunct
        ECA_DLCKREST = 38, // defunct

        ECA_SERVBEHIND = 39, // defunct
        ECA_NOCAST = 40, // defunct

        ECA_BADMASK = 41,

        ECA_IODONE = 42,
        ECA_IOINPROGRESS = 43,

        ECA_BADSYNCGRP = 44,
        ECA_PUTCBINPROG = 45,

        ECA_NORDACCESS = 46,
        ECA_NOWTACCESS = 47,

        ECA_ANACHRONISM = 48,

        ECA_NOSEARCHADDR = 49,
        ECA_NOCONVERT = 50,

        ECA_BADCHID = 51,
        ECA_BADFUNCPTR = 52,

        ECA_ISATTACHED = 53,
        ECA_UNAVAILINSERV = 54,

        ECA_CHANDESTROY = 55,
        ECA_BADPRIORITY = 56,

        ECA_NOTTHREADED = 57,

        ECA_16KARRAYCLIENT = 58,

        ECA_CONNSEQTMO = 59,
        ECA_UNRESPTMO = 60
    }

  // ?? There are lots of other ECA_ return codes
  // https://epics.anl.gov/EpicsDocumentation/AppDevManuals/ChannelAccess/cadoc_6.htm#MARKER-9-121
  //
  //   ECA_NORMAL          SUCCESS    
  //   
  //   ECA_IODONE          INFO      
  //   ECA_IOINPROGRESS    INFO      
  //   ECA_CHIDNOTFND      INFO       defunct
  //   ECA_CHIDRETRY       INFO       defunct
  //   ECA_NEWCONN         INFO       defunct
  //   
  //   ECA_ALLOCMEM        WARNING    
  //   ECA_TOLARGE         WARNING    
  //   ECA_TIMEOUT         WARNING   
  //   ECA_GETFAIL         WARNING   
  //   ECA_PUTFAIL         WARNING   
  //   ECA_BADCOUNT        WARNING   
  //   ECA_DISCONN         WARNING   
  //   ECA_DBLCHNL         WARNING   
  //   ECA_NORDACCESS      WARNING   
  //   ECA_NOWTACCESS      WARNING   
  //   ECA_NOSEARCHADDR    WARNING   
  //   ECA_NOCONVERT       WARNING   
  //   ECA_ISATTACHED      WARNING   
  //   ECA_UNAVAILINSERV   WARNING   
  //   ECA_CHANDESTROY     WARNING   
  //   ECA_16KARRAYCLIENT  WARNING   
  //   ECA_CONNSEQTMO      WARNING   
  //   ECA_UNRESPTMO       WARNING   
  //   ECA_CONN            WARNING    defunct
  //   ECA_UKNCHAN         WARNING    defunct
  //   ECA_UKNFIELD        WARNING    defunct
  //   ECA_NOSUPPORT       WARNING    defunct
  //   ECA_STRTOBIG        WARNING    defunct
  //   ECA_DBLCLFAIL       WARNING    defunct
  //   ECA_ADDFAIL         WARNING    defunct
  //   ECA_BUILDGET        WARNING    defunct
  //   ECA_NEEDSFP         WARNING    defunct
  //   ECA_OVEVFAIL        WARNING    defunct
  //   ECA_NEWADDR         WARNING    defunct
  //   ECA_NOCACTX         WARNING    defunct
  //   ECA_EMPTYSTR        WARNING    defunct
  //   ECA_NOREPEATER      WARNING    defunct
  //   ECA_NOCHANMSG       WARNING    defunct
  //   ECA_DLCKREST        WARNING    defunct
  //   ECA_SERVBEHIND      WARNING    defunct
  //   ECA_NOCAST          WARNING    defunct
  //   
  //   ECA_BADSTR          ERROR     
  //   ECA_BADTYPE         ERROR     
  //   ECA_EVDISALLOW      ERROR     
  //   ECA_BADMONID        ERROR     
  //   ECA_BADMASK         ERROR     
  //   ECA_BADSYNCGRP      ERROR     
  //   ECA_PUTCBINPROG     ERROR     
  //   ECA_ANACHRONISM     ERROR     
  //   ECA_BADCHID         ERROR     
  //   ECA_BADFUNCPTR      ERROR     
  //   ECA_BADPRIORITY     ERROR     
  //   ECA_NOTTHREADED     ERROR     
  //   ECA_MAXIOC          ERROR      defunct
  //   ECA_UKNHOST         ERROR      defunct
  //   ECA_UKNSERV         ERROR      defunct
  //   ECA_SOCK            ERROR      defunct
  //   ECA_DISCONNCHID     ERROR      defunct
  //   
  //   ECA_INTERNAL        FATAL   
  //   ECA_DEFUNCT         FATAL      defunct
  //   
  //   Hmm, can't seem to find a detailed description of these.
  //
  // Many are 'defunct', but there are a couple of dozen 'current' ones.
  // The docs indicate which ECA_ codes can be returned by each function.
  // For example ca_test_io() can return ECA_IODONE or ECA_IOINPROGRESS
  // BEST TO DEFINE THESE AS INTEGER CODES HERE ???
  // Note - a client might receive 'defunct' codes from an old IOC !!!
}
