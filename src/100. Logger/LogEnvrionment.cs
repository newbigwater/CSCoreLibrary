using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common._01._Definition;

namespace _100.Logger._01._Environment
{
    public class LogEnvironmentConfiguration
    {
        #region Properties

        public E_LOG_FILTER     LogFilterLevel                              { get; set; } // 디버그 로그를 출력할지 말지 선택
        public E_LOG_OUTPUT     LogOutputMode                               { get; set; } // 로그를 출력할 콘솔창/파일/둘다 중 선택
        public string           MaximumFileSize                             { get; set; } // 로그를 출력할 파일의 최대 사이즈
        public int              MaxSizeRoolBackups                          { get; set; } // MaximumFileSize를 초과했을 경우 백업할 로그 파일 수
        public string           LogFormat                                   { get; set; } // 출력 로그 형식
        public bool             EnableLogCollector                          { get; set; } // 로그를 삭제할 로그 삭제기 동작 여부
        public uint             LogCollectorOperationIntervalMinute         { get; set; } // 로그 삭제기 동작 간격
        public uint             LogRetentionPeriodDay                       { get; set; } // 로그 유지 기간
        public uint             LogDirectoryDriveMemoryMaximumUsagePercent  { get; set; }// 로그 디렉토리 존재 드라이브 최대 사용량

        #endregion

        #region Constructor

        public LogEnvironmentConfiguration
        (
            E_LOG_FILTER    filterlevel                              = E_LOG_FILTER.DEBUGING,
            E_LOG_OUTPUT    outputMode                               = E_LOG_OUTPUT.ALL,
            uint            retentionPeriod_Day                      = 30     ,
            uint            directoryDriveMemoryMaximumUsage_Percent = 100    ,
            bool            enableLogCollector                       = true   ,
            uint            logCollectorOperationIntervalMinute      = 60     ,
            string          maximumFileSize                          = "100MB",
            int             maxSizeRoolBackups                       = 100    ,
            string          logFormat                                = "[%date] [%-5level] [%thread] - %message%newline" // "[%logger] [%date] [%-5level] | %message%newline"
            )
        {
            LogFilterLevel                             = filterlevel;
            LogOutputMode                              = outputMode;
            MaximumFileSize                            = maximumFileSize;
            MaxSizeRoolBackups                         = maxSizeRoolBackups;
            LogFormat                                  = logFormat;
            EnableLogCollector                         = enableLogCollector;
            LogCollectorOperationIntervalMinute        = logCollectorOperationIntervalMinute * (1000 * 60);
            LogRetentionPeriodDay                      = retentionPeriod_Day;
            LogDirectoryDriveMemoryMaximumUsagePercent = directoryDriveMemoryMaximumUsage_Percent;
        }

        #endregion
    }
}
