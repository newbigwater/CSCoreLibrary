using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common._03._Attribute;

namespace _000.Common._01._Definition
{
    public enum E_ERROR_CODE
    {
        [EnumAttribute("Success", "성공")]
        SUCCESS                         ,

        // Stream Side.
        INVALID_STREAM                  = SUCCESS + 100,
        STREAM_READ_FAILURE             ,
        STREAM_WRITE_FAILURE            ,
        STREAM_END_OF_STREAM            ,

        END_STREAM_SIDE                 ,

        // File Side.
        INVALID_FILE                    = END_STREAM_SIDE + 100,
        FILE_ALREADY_EXIST              ,
        FILE_NOT_EXIST                  , 
        FILE_OPEN_FAILURE               ,
        FILE_INVALID_BOM_TYPE           ,
        FILE_END_OF_FILE                ,

        END_FILE_SIDE                   ,

        // Handle Side.
        INVALID_HANDLE                  = END_FILE_SIDE + 100,
        HANDLE_ALREADY_EXIST            ,
        HANDLE_NOT_EXIST                , 
        
        END_HANDLE_SIDE                 ,

        // State Side.
        INVALID_STATE                   = END_HANDLE_SIDE + 100,
        STATE_ALREADY_CREATED           ,
        STATE_NOT_CREATED               ,
        STATE_ALREADY_INITIALIZED       ,
        STATE_NOT_INITIALIZED           ,
        STATE_ALREADY_PREPARED          ,
        STATE_NOT_PREPARED              ,
        STATE_ALREADY_STARTED           ,
        STATE_NOT_STARTED               ,
        STATE_ALREADY_RUNNING           ,
        STATE_NOT_RUNNING               ,
        STATE_ALREADY_STOPPED           ,
        STATE_NOT_STOPPED               ,

        END_STATE_SIDE                  ,

        // Function Side.
        INVALID_FUNCTION                = END_STATE_SIDE + 100,
        FUNC_NOT_IMPLEMENTED            ,
        FUNC_NOT_SUPPORTED              ,

        END_FUNCTION_SIDE               ,

        // Event Side.
        INVALID_EVENT                   = END_FUNCTION_SIDE + 100,
        EVENT_CALL_FAILURE              ,
        EVENT_ALREADY_REQUESTED         ,
        EVENT_NOT_REQUESTED             ,
        EVENT_ALREADY_PROCESSED         ,
        EVENT_NOT_PROCESSED             ,
        EVENT_ALREADY_RESPONDED         ,
        EVENT_NOT_RESPONDED             ,
        EVENT_ALREADY_REPORTED          ,
        EVENT_NOT_REPORTED              ,
        EVENT_TIME_OUT                  ,
        EVENT_CANCLE                    ,
        EVENT_ABANDONED                 ,
        
        END_EVENT_SIDE                  ,

        // Network Side.
        INVALID_NETWORK                 = END_EVENT_SIDE + 100,
        NETWORK_ALREADY_OPENED          ,
        NETWORK_NOT_OPENED              ,
        NETWORK_ALREADY_CONNECTED       ,
        NETWORK_NOT_CONNECTED           ,
        NETWORK_ALREADY_DISCONNECTED    ,
        NETWORK_NOT_DISCONNECTED        ,
        NETWORK_LIMIT_CONNECTION        ,
        NETWORK_INVALID_IP              ,
        NETWORK_INVALID_PORT            ,
        NETWORK_SEND_FAILURE            ,
        NETWORK_RECV_FAILURE            ,

        END_NETWORK_SIDE                ,

        // Data Side.
        INVALID_DATA                    = END_NETWORK_SIDE + 100,
        DATA_NOT_ENOUGH_MEMORY          ,
        DATA_NO_DATA                    ,
        DATA_MORE_DATA                  ,
        DATA_INVALID_BLOCK              ,
        DATA_OUT_OF_RANGE               ,

        END_DATA_SIDE                   ,

        // ETC Side.
        [EnumAttribute("ETC", "알 수 없는 내부 에러가 발생했습니다.")]
        INTERNAL_ERROR                  = END_DATA_SIDE + 100
    }
}
