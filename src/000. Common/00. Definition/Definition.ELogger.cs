using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _000.Common._01._Definition
{
    public enum E_LOG_LEVEL
    {
        DEBUG   = 0,
        INFO    ,
        WARN    ,
        ERROR   ,
        FATAL   
    }

    public enum E_LOG_FILTER
    {
        DEBUGING    = E_LOG_LEVEL.DEBUG,
        NORMAL      = E_LOG_LEVEL.INFO,
    }

    [Flags]
    public enum E_LOG_APPENDER
    {
        NONE        = 0,
        ONLY_FATAL  ,
        ONLY_NORMAL , 
        ALL         = ONLY_FATAL + ONLY_NORMAL
    }

    public enum E_LOG_OUTPUT
    {
        NONE    = 0,
        CONSOLE ,
        FILE    ,
        ALL     = CONSOLE + FILE
    }
}
