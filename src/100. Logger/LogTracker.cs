using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using _000.Common;
using _000.Common._01._Definition;
using _000.Common._04._Struct;
using _000.Common._05._Class;
using _100.Logger._01._Environment;
using _100.Logger._01._Interface;
using log4net;
using log4net.Core;

namespace _100.Logger
{
    /********************************************************
    * @date 2019-08-15 18:22:47
    * @author newbigwater@gmail.com
    * @brief  Logger
    * @details Instance의 이름을 전달받아 해당 이름으로 로그를 출력한다.
    * @version
    *********************************************************/
    public class LogTracker : IDisposable
    {
        #region Fields
        
        private static readonly Object _locker = new Object();
        private static readonly Lazy<LogTracker> _instance = new Lazy<LogTracker>(() => new LogTracker());


        private LogEnvironmentConfiguration _configure;
        private readonly log4net.Repository.Hierarchy.Hierarchy _hierarchy;

        private LogDevice _mainLogDevice = null;
        private Dictionary<string, LogDevice> _dicDevices = new Dictionary<string, LogDevice>();

        private System.Threading.Timer _logCollector = null;

        #endregion

        #region Properties

        public static Object Locker => _locker;
        public static LogTracker Instance => _instance.Value;

        public LogEnvironmentConfiguration Configure => _configure;

        public LogDevice MainLogDevice => _mainLogDevice;


        private bool Activation => _hierarchy == null ? false : _hierarchy.Configured;

        #endregion

        #region Constructor

        private LogTracker()
        {
            string strLogging = $"[{MethodBase.GetCurrentMethod().Name}]";
            try
            {
                try
                {
                    //_hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository(Assembly.GetEntryAssembly());
                    _hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository("LogTracker");
                }
                catch (Exception exp)
                {
                    _hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.CreateRepository("LogTracker");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _hierarchy.Threshold = log4net.Core.Level.All;
        }

        public void Dispose()
        {
            _logCollector?.Dispose();
        }

        #endregion

        #region Accessor

        /// <summary>
        /// Set log tracking device configuration information.
        /// </summary>
        /// <param name="configure">Environment configuration</param>
        /// <param name="directory">Log output path.</param>
        /// <returns>Success</returns>
        public static E_ERROR_CODE Initialize(LogEnvironmentConfiguration configure = null, string directory = "Logs")
        {
            try
            {
                if (Instance.Activation)
                    return E_ERROR_CODE.STATE_ALREADY_INITIALIZED;

                lock (Locker)
                {
                    if (null == Instance._configure)
                    {
                        if (null == configure)
                            configure = new LogEnvironmentConfiguration();

                        Instance._configure = configure;
                    }
                }

                Instance.GenerateMainLogDevice(directory);

                lock (Locker)
                {
                    if (configure.EnableLogCollector)
                        Instance._logCollector = new System.Threading.Timer(Instance.LogCollection, (object)Instance.Configure, 1000, Instance.Configure.LogCollectorOperationIntervalMinute);

                    ChangeLogFilterLevel(Instance.Configure.LogFilterLevel);
                }
            }
            catch (Exception e)
            {
                Log_Fatal(e);
                throw e;
            }

            return E_ERROR_CODE.SUCCESS;
        }

        /// <summary>
        /// Main log tracking device.
        /// </summary>
        /// <param name="directory">Log output path.</param>
        private void GenerateMainLogDevice(string directory = "Logs")
        {
            try
            {
                lock (Locker)
                {
                    // DCL
                    if (null != _mainLogDevice)
                        return;

                    if (!directory.Contains(@":\"))
                        directory = $@"{System.IO.Directory.GetCurrentDirectory()}\{directory}";

                    var name = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".vshost", "");

                    var logger = _hierarchy.LoggerFactory.CreateLogger((log4net.Repository.ILoggerRepository)_hierarchy, name);
                    logger.Hierarchy = _hierarchy;
                    logger.Level = log4net.Core.Level.All;

                    _mainLogDevice = new LogDevice(name, directory, ref logger);
                    CheckLogAppenderDateUpdate(_mainLogDevice);

                    _hierarchy.Configured = true;
                }
            }
            catch (Exception e)
            {
                Log_Fatal(e);
                throw e;
            }
        }

        /// <summary>
        /// Parent log tracker chained log output tracker creation function.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="parentDevice">The parent log chain device.(If the corresponding variable is null, it is connected to the main log tracking device.)</param>
        /// <param name="directory">Log output path.</param>
        /// <param name="isLogWithoutIntermediateDirectory">Log Without Intermediate Directory.</param>
        /// <returns>log tracking device.</returns>
        public static LogDevice GetLogDevice(string name, ILogDevice parentDevice = null, string directory = "", bool isLogWithoutIntermediateDirectory = false)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            LogDevice device = null;
            try
            {
                string fileDir = Instance._mainLogDevice.BaseDirectory;
                if (!string.IsNullOrWhiteSpace(directory))
                    fileDir += $@"\{directory}";

                lock (Locker)
                {
                    if (Instance._dicDevices.ContainsKey($@"{fileDir}\{name}"))
                        return Instance._dicDevices[$@"{fileDir}\{name}"];

                    var logger = Instance._hierarchy.LoggerFactory.CreateLogger((log4net.Repository.ILoggerRepository)Instance._hierarchy, name);
                    logger.Hierarchy = Instance._hierarchy;
                    logger.Level = GetLogLevel();
                    if (null == parentDevice)
                        device = new LogDevice(name, fileDir, ref logger, Instance._mainLogDevice)
                        {
                            IsLogWithoutIntermediateDirectory = isLogWithoutIntermediateDirectory
                        };
                    else
                        device = new LogDevice(name, fileDir, ref logger, parentDevice)
                        {
                            IsLogWithoutIntermediateDirectory = isLogWithoutIntermediateDirectory
                        };
                    CheckLogAppenderDateUpdate(device);
                    Instance._dicDevices.Add($@"{fileDir}\{name}", device);
                }
            }
            catch (Exception e)
            {
                Log_Fatal(e);
            }

            return device;
        }

        /// <summary>
        /// Single log output tracker device creation function.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="directory">Log output path.</param>
        /// <param name="isLogWithoutIntermediateDirectory">Log Without Intermediate Directory.</param>
        /// <returns>log tracking device.</returns>
        public static LogDevice GetSingleLogDevice(string name, string directory = "", bool isLogWithoutIntermediateDirectory = false)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            LogDevice device = null;
            try
            {
                string fileDir = Instance._mainLogDevice.BaseDirectory;
                if (!string.IsNullOrWhiteSpace(directory))
                    fileDir += $@"\{directory}";

                lock (Locker)
                {
                    if (Instance._dicDevices.ContainsKey($@"{fileDir}\{name}"))
                        return Instance._dicDevices[$@"{fileDir}\{name}"];

                    var logger = Instance._hierarchy.LoggerFactory.CreateLogger((log4net.Repository.ILoggerRepository)Instance._hierarchy, name);
                    logger.Hierarchy = Instance._hierarchy;
                    logger.Level = GetLogLevel();

                    device = new LogDevice(name, fileDir, ref logger)
                    {
                        IsLogWithoutIntermediateDirectory = isLogWithoutIntermediateDirectory
                    };
                    CheckLogAppenderDateUpdate(device);
                    Instance._dicDevices.Add($@"{fileDir}\{name}", device);
                }
            }
            catch (Exception e)
            {
                Log_Fatal(e);
            }

            return device;
        }

        /// <summary>
        /// Normal log output tracker device creation function.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="directory">Log output path.</param>
        /// <param name="parentDevice">The parent log chain device.(If the corresponding variable is null, it is connected to the main log tracking device.)</param>
        /// <param name="isLogWithoutIntermediateDirectory">Log Without Intermediate Directory.</param>
        /// <returns>log tracking device.</returns>
        public static LogDevice GetNormalLogDevice(string name, string directory = "", LogDevice parentDevice = null, bool isLogWithoutIntermediateDirectory = false)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            LogDevice device = null;
            try
            {
                string fileDir = Instance._mainLogDevice.BaseDirectory;
                if (!string.IsNullOrWhiteSpace(directory))
                    fileDir += $@"\{directory}";

                lock (Locker)
                {
                    if (Instance._dicDevices.ContainsKey($@"{fileDir}\{name}"))
                        return Instance._dicDevices[$@"{fileDir}\{name}"];

                    var logger = Instance._hierarchy.LoggerFactory.CreateLogger((log4net.Repository.ILoggerRepository)Instance._hierarchy, name);
                    logger.Hierarchy = Instance._hierarchy;
                    logger.Level = GetLogLevel();

                    device = new LogDevice(name, fileDir, ref logger, parentDevice)
                    {
                        IsAppender = E_LOG_APPENDER.ONLY_NORMAL,
                        IsLogWithoutIntermediateDirectory = isLogWithoutIntermediateDirectory
                    };
                    CheckLogAppenderDateUpdate(device);
                    Instance._dicDevices.Add($@"{fileDir}\{name}", device);
                }
            }
            catch (Exception e)
            {
                Log_Fatal(e);
            }

            return device;
        }

        /// <summary>
        /// Fatal log output tracker device creation function.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="directory">Log output path.</param>
        /// <param name="parentDevice">The parent log chain device.(If the corresponding variable is null, it is connected to the main log tracking device.)</param>
        /// <param name="isLogWithoutIntermediateDirectory">Log Without Intermediate Directory.</param>
        /// <returns>log tracking device.</returns>
        public static LogDevice GetFatalLogDevice(string name, string directory = "", LogDevice parentDevice = null, bool isLogWithoutIntermediateDirectory = false)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            LogDevice device = null;
            try
            {
                string fileDir = Instance._mainLogDevice.BaseDirectory;
                if (!string.IsNullOrWhiteSpace(directory))
                    fileDir += $@"\{directory}";

                lock (Locker)
                {
                    if (Instance._dicDevices.ContainsKey($@"{fileDir}\{name}"))
                        return Instance._dicDevices[$@"{fileDir}\{name}"];

                    var logger = Instance._hierarchy.LoggerFactory.CreateLogger((log4net.Repository.ILoggerRepository)Instance._hierarchy, name);
                    logger.Hierarchy = Instance._hierarchy;
                    logger.Level = Level.Fatal;

                    device = new LogDevice(name, fileDir, ref logger, parentDevice)
                    {
                        IsAppender = E_LOG_APPENDER.ONLY_FATAL,
                        IsLogWithoutIntermediateDirectory = isLogWithoutIntermediateDirectory
                    };
                    CheckLogAppenderDateUpdate(device);
                    Instance._dicDevices.Add($@"{fileDir}\{name}", device);
                }
            }
            catch (Exception e)
            {
                Log_Fatal(e);
            }

            return device;
        }

        /// <summary>
        /// Current log output level.
        /// </summary>
        /// <returns>Current log output level.</returns>
        public static log4net.Core.Level GetLogLevel()
        {
            try
            {
                if (Instance._hierarchy.Threshold == Level.Debug)
                    return log4net.Core.Level.Debug;
                else if (Instance._hierarchy.Threshold == Level.Info)
                    return log4net.Core.Level.Info;
                else if (Instance._hierarchy.Threshold == Level.Warn)
                    return log4net.Core.Level.Warn;
                else if (Instance._hierarchy.Threshold == Level.Fatal)
                    return log4net.Core.Level.Fatal;
                else if (Instance._hierarchy.Threshold == Level.Error)
                    return log4net.Core.Level.Error;
            }
            catch (Exception e)
            {
                Log_Fatal(e);
            }

            return log4net.Core.Level.All;
        }

        /// <summary>
        /// Fatal log output Main log tracking device.
        /// </summary>
        /// <param name="strMessage">Fatal log text</param>
        public static void Log_Fatal(string strMessage)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Fatal(strMessage);
        }

        /// <summary>
        /// Exception log output Main log tracking device.
        /// </summary>
        /// <param name="ex">Exception object</param>
        public static void Log_Fatal(Exception ex)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Fatal(ex);
        }

        /// <summary>
        /// Fatal log Message, Exception Position Log output Main log tracking device.
        /// </summary>
        /// <param name="strMessage">Fatal log text</param>
        /// <param name="strMessage">Exception object</param>
        public static void Log_Fatal(string strMessage, Exception ex)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Fatal(strMessage, ex);
        }

        /// <summary>
        /// Error log output Main log tracking device.
        /// </summary>
        /// <param name="strMessage">Error log text.</param>
        public static void Log_Error(string strMessage)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Error(strMessage);
        }

        /// <summary>
        /// Warning log output Main log tracking device.
        /// </summary>
        /// <param name="strMessage">Warning log text.</param>
        public static void Log_Warn(string strMessage)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Warn(strMessage);
        }

        /// <summary>
        /// Information log output Main log tracking device.
        /// </summary>
        /// <param name="strMessage">Information log text.</param>
        public static void Log_Info(string strMessage)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Info(strMessage);
        }

        /// <summary>
        /// Debug log output Main log tracking device.
        /// </summary>
        /// <param name="strMessage">Debug log text.</param>
        public static void Log_Debug(string strMessage)
        {
            if (null == Instance._mainLogDevice && E_ERROR_CODE.SUCCESS != Initialize())
                throw new UserException($"Log Tracker does not initialize.", E_ERROR_CODE.STATE_NOT_INITIALIZED);

            Instance?._mainLogDevice.Log_Debug(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="onlyFatal">Only Fatal log tracking device mode.</param>
        public static void Log_Fatal(string name, string strMessage, bool onlyFatal = false)
        {
            LogDevice device = null;
            if(onlyFatal)
                device = GetFatalLogDevice(name);
            else
                device = GetSingleLogDevice(name);
            device?.Log_Fatal(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="ex">Exception object</param>
        /// <param name="onlyFatal">Only Fatal log tracking device mode.</param>
        public static void Log_Fatal(string name, Exception ex, bool onlyFatal = false)
        {
            LogDevice device = null;
            if (onlyFatal)
                device = GetFatalLogDevice(name);
            else
                device = GetSingleLogDevice(name);
            device?.Log_Fatal(ex);
        }

        /// <summary>
        /// Fatal log Message, Exception Position Log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="ex">Exception object</param>
        /// <param name="onlyFatal">Only Fatal log tracking device mode.</param>
        public static void Log_Fatal(string name, string strMessage, Exception ex, bool onlyFatal = false)
        {
            LogDevice device = null;
            if (onlyFatal)
                device = GetFatalLogDevice(name);
            else
                device = GetSingleLogDevice(name);
            device?.Log_Fatal(strMessage, ex);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        public static void Log_Error(string name, string strMessage)
        {
            LogDevice device = GetSingleLogDevice(name);
            device?.Log_Error(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        public static void Log_Warn(string name, string strMessage)
        {
            LogDevice device = GetSingleLogDevice(name);
            device?.Log_Warn(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        public static void Log_Info(string name, string strMessage)
        {
            LogDevice device = GetSingleLogDevice(name);
            device?.Log_Info(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        public static void Log_Debug(string name, string strMessage)
        {
            LogDevice device = GetSingleLogDevice(name);
            device?.Log_Debug(strMessage);
        }

#region Legacy

        /*
           /// <summary>
           /// Fatal log output Main log tracking device.
           /// </summary>
           /// <param name="strMessage">Fatal log text</param>
           public static void FatalLog(string strMessage) => Instance?.Log_Fatal(strMessage);

           /// <summary>
           /// Exception log output Main log tracking device.
           /// </summary>
           /// <param name="ex">Exception object</param>
           public static void FatalExcLogHandle(Exception ex) => Instance?.Log_FatalExc(ex);

           /// <summary>
           /// Fatal log Message, Exception Position Log output Main log tracking device.
           /// </summary>
           /// <param name="strMessage">Fatal log text</param>
           /// <param name="ex">Exception object</param>
           public static void FatalExtLogHandle(string strMessage, Exception ex) => Instance?.Log_FatalExt(strMessage, ex);

           /// <summary>
           /// Error log output Main log tracking device.
           /// </summary>
           /// <param name="strMessage">Error log text.</param>
           public static void ErrorLogHandle(string strMessage) => Instance?.Log_Error(strMessage);

           /// <summary>
           /// Warning log output Main log tracking device.
           /// </summary>
           /// <param name="strMessage">Warning log text.</param>
           public static void WarnLogHandle(string strMessage) => Instance?.Log_Warn(strMessage);

           /// <summary>
           /// Information log output Main log tracking device.
           /// </summary>
           /// <param name="strMessage">Information log text.</param>
           public static void InfoLogHandle(string strMessage) => Instance?.Log_Info(strMessage);

           /// <summary>
           /// Debug log output Main log tracking device.
           /// </summary>
           /// <param name="strMessage">Debug log text.</param>
           public static void DebugLogHandle(string strMessage) => Instance?.Log_Debug(strMessage);
         */

#endregion

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="dir">Log output path.</param>
        /// <param name="onlyFatal">Only Fatal log tracking device mode.</param>
        public static void FatalLog(string name, string strMessage, string dir = "", bool onlyFatal = false)
        {
            LogDevice device = null;
            if (onlyFatal)
                LogTracker.GetFatalLogDevice(name, dir);
            else
                LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Fatal(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="ex">Exception object</param>
        /// <param name="dir">Log output path.</param>
        /// <param name="onlyFatal">Only Fatal log tracking device mode.</param>
        public static void FatalExcLog(string name, Exception ex, string dir = "", bool onlyFatal = false)
        {
            LogDevice device = null;
            if (onlyFatal)
                LogTracker.GetFatalLogDevice(name, dir);
            else
                LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Fatal(ex);
        }

        /// <summary>
        /// Fatal log Message, Exception Position Log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="ex">Exception object</param>
        /// <param name="dir">Log output path.</param>
        /// <param name="onlyFatal">Only Fatal log tracking device mode.</param>
        public static void FatalExtLog(string name, string strMessage, Exception ex, string dir = "", bool onlyFatal = false)
        {
            LogDevice device = null;
            if(onlyFatal)
                LogTracker.GetFatalLogDevice(name, dir);
            else
                LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Fatal(strMessage, ex);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="dir">Log output path.</param>
        public static void ErrorLog(string name, string strMessage, string dir = "")
        {
            LogDevice device = LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Error(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="dir">Log output path.</param>
        public static void WarnLog(string name, string strMessage, string dir = "")
        {
            LogDevice device = LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Warn(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="dir">Log output path.</param>
        public static void InfoLog(string name, string strMessage, string dir = "")
        {
            LogDevice device = LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Info(strMessage);
        }

        /// <summary>
        /// Fatal log output Single log tracking device.
        /// </summary>
        /// <param name="name">Logger Identifier.</param>
        /// <param name="strMessage">Fatal log text.</param>
        /// <param name="dir">Log output path.</param>
        public static void DebugLog(string name, string strMessage, string dir = "")
        {
            LogDevice device = LogTracker.GetSingleLogDevice(name, dir);
            device?.Log_Debug(strMessage);
        }

        private void LogCollection(object state)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var baseDate = DateTime.Today.AddDays(-Configure.LogRetentionPeriodDay).ToString("yyyy-MM-dd");
                    string guid = Guid.NewGuid().ToString().ToUpper();
                    string log =
                        $"\n" +
                        $"-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n" +
                        $"-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= Log Collection Start =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n" +
                        $"-= Base  Date : {baseDate} =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-\n" +
                        $"-= {guid} -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-";
                    Log_Info(log);
                    log =
                        $"\n" +
                        $"-= {guid} -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-\n" +
                        $"-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-= Log Collection Ended =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n" +
                        $"-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-\n";

                    LogCollection(baseDate.Replace("-", ""), _mainLogDevice.BaseDirectory);

                    ExtDriveInfo info = Common.GetDriveSize(_mainLogDevice.BaseDirectory);
                    if (Configure.LogDirectoryDriveMemoryMaximumUsagePercent < info.UsagePercent)
                        LogCollectionForDriveUsage(_mainLogDevice.BaseDirectory);

                    Log_Info(log);
                }
                catch (Exception e)
                {
                    Log_Fatal($"[{e.Source}] [{e?.GetType()?.FullName}] {e.Message}\n{e.StackTrace}");
                }
            });
        }

        private void LogCollectionForDriveUsage(string baseDir)
        {
            Task.Factory.StartNew(() =>
            {
                if (!Activation)
                    return;

                uint logRetentionPeriodDay = Configure.LogRetentionPeriodDay;

                ExtDriveInfo info = Common.GetDriveSize(_mainLogDevice.BaseDirectory);
                while (Configure.LogDirectoryDriveMemoryMaximumUsagePercent < info.UsagePercent)
                {
                    if (!Activation)
                        return;

                    var baseDate = DateTime.Today.AddDays(-(logRetentionPeriodDay--)).ToString("yyyy-MM-dd");
                    if (0 == String.Compare(baseDate, Path.GetFileName(DateTime.Today.ToString("yyyy-MM-dd")),
                            StringComparison.Ordinal))
                    {
                        Log_Fatal($"{info.Name} drive usage problem, but there are no more logs to delete.");
                        return;
                    }

                    LogCollection(baseDate, _mainLogDevice.BaseDirectory);
                }
            });
        }

        private void LogCollection(string baseDate, string baseDirectory, int depth = 1)
        {
            if (!Activation || 10 == depth)
                return;

            foreach (var directory in Directory.GetDirectories(baseDirectory))
            {
                if (!Activation)
                    return;

                if (0 < String.Compare(baseDate, Path.GetFileName(directory), StringComparison.Ordinal))
                {
                    try
                    {
                        Log_Debug($"The [{directory}] Directory has been deleted.");
                        Directory.Delete(directory, true);
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                    LogCollection(baseDate, directory, depth + 1);
            }

            foreach (var file in Directory.GetFiles(baseDirectory))
            {
                try
                {
                    if (!Activation)
                        return;

                    var fi = new FileInfo(file);
                    if (0 <= String.Compare(baseDate, fi.LastWriteTime.ToString("yyyyMMdd"), StringComparison.Ordinal))
                    {
                        Log_Debug($"The [{fi.FullName}] file has been deleted.");
                        File.Delete(fi.FullName);
                    }
                }
                catch (Exception e)
                {
                }
            }

            if (0 == Directory.GetDirectories(baseDirectory).Length && 0 == Directory.GetFiles(baseDirectory).Length)
            {
                try
                {
                    Directory.Delete(baseDirectory);
                }
                catch (Exception exp)
                {
                }
            }
        }

        #endregion

        #region Mutator

        /// <summary>
        /// 디버깅 로그 출력 여부 설정 기능입니다.
        /// </summary>
        /// <param name="logFilterLevel">NORMAL Mode does not output debugging logs.
        /// <br/> Enum Type : <see cref="E_LOG_FILTER" />
        /// </param>
        /// <summary>
        /// Build System의 Batch file에 의해 빌드된 Assembly의 <see cref="Version"/>정보를 기반으로 빌드된 시간에 대한 <see cref="T:System.DateTime" /> 개체를 반환합니다.
        /// </summary>
        public static void ChangeLogFilterLevel(E_LOG_FILTER logFilterLevel)
        {
            switch (logFilterLevel)
            {
                case E_LOG_FILTER.NORMAL: Instance._hierarchy.Threshold = log4net.Core.Level.Info   ; break;
                default                 : Instance._hierarchy.Threshold = log4net.Core.Level.All    ; break;
            }
            Instance._hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }

        /// <summary>
        /// This is the log output level setting function.
        /// </summary>
        /// <param name="logLevel">Debug -> Info -> Warn -> Error -> Fatal</param>
        public static void ChangeLogLevel(E_LOG_LEVEL logLevel)
        {
            switch (logLevel)
            {
                case E_LOG_LEVEL.DEBUG  :   Instance._hierarchy.Threshold = log4net.Core.Level.Debug; break;
                case E_LOG_LEVEL.INFO   :   Instance._hierarchy.Threshold = log4net.Core.Level.Info ; break;
                case E_LOG_LEVEL.WARN   :   Instance._hierarchy.Threshold = log4net.Core.Level.Warn ; break;
                case E_LOG_LEVEL.ERROR  :   Instance._hierarchy.Threshold = log4net.Core.Level.Error; break;
                case E_LOG_LEVEL.FATAL  :   Instance._hierarchy.Threshold = log4net.Core.Level.Fatal; break;
                default                 :   Instance._hierarchy.Threshold = log4net.Core.Level.Info ; break;
            }
            Instance._hierarchy.RaiseConfigurationChanged(EventArgs.Empty);
        }

        /// <summary>
        /// This is a function to set the log retention period.
        /// </summary>
        /// <param name="retentionPeriod_Day">log retention days</param>
        public static void ChangeLogRetentionPeriodDay(uint retentionPeriod_Day = 30)
        {
            if (null == Instance.Configure)
                Instance._configure = new LogEnvironmentConfiguration();
            Instance.Configure.LogRetentionPeriodDay = retentionPeriod_Day;
        }


        /// <summary>
        /// The log output date directory check and reset function.
        /// </summary>
        /// <param name="device">Current log tracking device.</param>
        public static void CheckLogAppenderDateUpdate(LogDevice device)
        {
            string todayDirectory = DateTime.Now.Date.ToString("yyyy-MM-dd");
            if (String.Compare(device.TodayDirectory, todayDirectory, StringComparison.Ordinal) != 0)
            {
                device.Enter();
                {
                    if (String.Compare(device.TodayDirectory, todayDirectory, StringComparison.Ordinal) == 0)
                        return;

                    device.TodayDirectory = todayDirectory;
                    SetLogOutputAppender(device);
                }
                device.Leave();
            }
        }

        /// <summary>
        /// This is the log output environment setting function.
        /// </summary>
        /// <param name="device"><Current log tracking device.</param>
        private static void SetLogOutputAppender(LogDevice device)
        {
            device.DeviceLogger.RemoveAllAppenders();
            switch (Instance.Configure.LogOutputMode)
            {
                case E_LOG_OUTPUT.CONSOLE: Instance.LogOutput_Console(device); break;
                case E_LOG_OUTPUT.FILE: Instance.LogOutput_File(device); break;
                default:
                    {
                        Instance.LogOutput_Console(device);
                        Instance.LogOutput_File(device);
                    }
                    break;
            }
        }

        /// <summary>
        /// This is the log console window output environment setting function.
        /// </summary>
        /// <param name="device"><Current log tracking device.</param>
        private void LogOutput_Console(LogDevice device)
        {
            log4net.Appender.ConsoleAppender rollingAppender_Console = new log4net.Appender.ConsoleAppender();
            {
                rollingAppender_Console.Layout = new log4net.Layout.PatternLayout(Configure.LogFormat);
                rollingAppender_Console.ActivateOptions();
            }
            device.DeviceLogger.AddAppender(rollingAppender_Console);
        }

        /// <summary>
        /// This is the log file output environment setting function.
        /// </summary>
        /// <param name="device"><Current log tracking device.</param>
        /// 
        private void LogOutput_File(LogDevice device)
        {
            if (!device.IsLogWithoutIntermediateDirectory)
            {
                // 로그 폴더 생성.
                string dateDir = DateTime.Now.Date.ToString("yyyyMMdd");
                if(device.IsAppender.HasFlag(E_LOG_APPENDER.ONLY_NORMAL))
                {
                    log4net.Appender.RollingFileAppender rollingAppender_File = new log4net.Appender.RollingFileAppender();
                    {
                        rollingAppender_File.File = $@"{device.BaseDirectory}\{device.Name}\Logs\";
                        rollingAppender_File.DatePattern = $"{dateDir}/\"{device.Name}.\"HH\".log\"";
                        rollingAppender_File.StaticLogFileName = false;
                        rollingAppender_File.AppendToFile = true;
                        rollingAppender_File.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Composite;
                        rollingAppender_File.MaxSizeRollBackups = Configure.MaxSizeRoolBackups;
                        rollingAppender_File.MaximumFileSize = Configure.MaximumFileSize;
                        rollingAppender_File.Layout = new log4net.Layout.PatternLayout(Configure.LogFormat);
                        rollingAppender_File.ActivateOptions();
                    }
                    device.DeviceLogger.AddAppender(rollingAppender_File);
                }
                    
                if (device.IsAppender.HasFlag(E_LOG_APPENDER.ONLY_FATAL))
                {
                    log4net.Appender.RollingFileAppender rollingAppender_File_Fatal = new log4net.Appender.RollingFileAppender();
                    {
                        rollingAppender_File_Fatal.File = $@"{device.BaseDirectory}\{device.Name}\Logs.Fatal\";
                        rollingAppender_File_Fatal.DatePattern = $"\"{device.Name}.FATAL.\"{dateDir}.HH\".log\"";
                        rollingAppender_File_Fatal.StaticLogFileName = false;
                        rollingAppender_File_Fatal.AppendToFile = true;
                        rollingAppender_File_Fatal.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Composite;
                        rollingAppender_File_Fatal.MaxSizeRollBackups = Configure.MaxSizeRoolBackups;
                        rollingAppender_File_Fatal.MaximumFileSize = Configure.MaximumFileSize;
                        rollingAppender_File_Fatal.Layout = new log4net.Layout.PatternLayout(Configure.LogFormat);
                        {
                            log4net.Filter.LevelRangeFilter filter = new log4net.Filter.LevelRangeFilter();
                            filter.LevelMin = log4net.Core.Level.Fatal;
                            filter.LevelMax = log4net.Core.Level.Fatal;

                            rollingAppender_File_Fatal.AddFilter(filter);
                        }
                        rollingAppender_File_Fatal.ActivateOptions();
                    }
                    device.DeviceLogger.AddAppender(rollingAppender_File_Fatal);
                }
            }
            else
            {
                if (device.IsAppender.HasFlag(E_LOG_APPENDER.ONLY_NORMAL))
                {
                    log4net.Appender.RollingFileAppender rollingAppender_File = new log4net.Appender.RollingFileAppender();
                    {
                        rollingAppender_File.File = $@"{device.BaseDirectory}\{device.Name}.log";
                        rollingAppender_File.AppendToFile = true;
                        rollingAppender_File.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                        rollingAppender_File.MaxSizeRollBackups = Configure.MaxSizeRoolBackups;
                        rollingAppender_File.MaximumFileSize = Configure.MaximumFileSize;
                        rollingAppender_File.Layout = new log4net.Layout.PatternLayout(Configure.LogFormat);
                        rollingAppender_File.ActivateOptions();
                    }
                    device.DeviceLogger.AddAppender(rollingAppender_File);
                }

                if (device.IsAppender.HasFlag(E_LOG_APPENDER.ONLY_FATAL))
                {
                    log4net.Appender.RollingFileAppender rollingAppender_File_Fatal = new log4net.Appender.RollingFileAppender();
                    {
                        rollingAppender_File_Fatal.File = $@"{device.BaseDirectory}\{device.Name}.FATAL.log";
                        rollingAppender_File_Fatal.AppendToFile = true;
                        rollingAppender_File_Fatal.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                        rollingAppender_File_Fatal.MaxSizeRollBackups = Configure.MaxSizeRoolBackups;
                        rollingAppender_File_Fatal.MaximumFileSize = Configure.MaximumFileSize;
                        rollingAppender_File_Fatal.Layout = new log4net.Layout.PatternLayout(Configure.LogFormat);
                        {
                            log4net.Filter.LevelRangeFilter filter = new log4net.Filter.LevelRangeFilter();
                            filter.LevelMin = log4net.Core.Level.Fatal;
                            filter.LevelMax = log4net.Core.Level.Fatal;

                            rollingAppender_File_Fatal.AddFilter(filter);
                        }
                        rollingAppender_File_Fatal.ActivateOptions();
                    }
                    device.DeviceLogger.AddAppender(rollingAppender_File_Fatal);
                }
            }
        }

        #endregion

        #region Predicate

        #endregion
    }
}
