using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IPA.LoggerManager
{
    public enum LOGLEVELS
    {
        NONE    = 0x00,
        FATAL   = 0x01,
        ERROR   = 0x02,
        WARNING = 0x04,
        INFO    = 0x08,
        DEBUG   = 0x10,
        ALL     = 0x1F
    }
    public static class LogLevels
    {
        public static Dictionary<LOGLEVELS, string> LogLevelsDictonary = new Dictionary<LOGLEVELS, string>() 
        {
            { LOGLEVELS.NONE   , "NONE"    },
            { LOGLEVELS.FATAL  , "FATAL"   },
            { LOGLEVELS.ERROR  , "ERROR"   },
            { LOGLEVELS.WARNING, "WARNING" },
            { LOGLEVELS.INFO   , "INFO"    },
            { LOGLEVELS.DEBUG  , "DEBUG"   }
        };
    }
}
