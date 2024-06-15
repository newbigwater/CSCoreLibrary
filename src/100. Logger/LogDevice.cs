using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _000.Common._01._Definition;
using _000.Common._05._Class;
using _100.Logger._01._Interface;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace _100.Logger
{
    public sealed class LogDevice : ILogDevice, IDisposable
    {
        #region ▶  Event Handle             ◀

        public event LogDebugHandler    DebugLogHandle      ;
        public event LogInfoHandler     InfoLogHandle       ;
        public event LogWarnHandler     WarnLogHandle       ;
        public event LogErrorHandler    ErrorLogHandle      ;
        public event LogFatalHandler    FatalLogHandle      ;
        public event LogFatalExcHandler FatalExcLogHandle   ;
        public event LogFatalExtHandler FatalExtLogHandle   ;

        #endregion // Event Handle

        #region ▶  Fields                   ◀

        private readonly object    _lock             = new object();
        private readonly Semaphore _semaphore        = new Semaphore(1, 1);
        private bool               _isAccessible     = true;
        private string             _callerMemberName = "";

        private readonly ILogDevice _parentLogger   = null;
        private readonly string     _directory      = "";
        private readonly string     _name           = "";
        private string              _todayDirectory = "";

        private readonly log4net.Repository.Hierarchy.Logger    _logger = null;
        private readonly ILog                                   _log    = null;

        private E_LOG_APPENDER _appenderType = E_LOG_APPENDER.ALL;

        #endregion // Fields

        #region ▶  Properties               ◀

        public bool IsAccessible
        {
            get
            {
                lock (_lock)
                {
                    return _isAccessible;
                }
            }
        }

        public void Enter(string memberName = "")
        {
            _semaphore.WaitOne();
            lock (_lock)
            {
                _isAccessible = false;
                _callerMemberName = memberName;
            }
        }

        public void Leave(string memberName = "")
        {
            if (IsAccessible)
                return;

            lock (_lock)
            {
                if (!_callerMemberName.Equals(memberName))
                    return;

                _isAccessible = true;
                _callerMemberName = "";
            }
            _semaphore.Release();
        }

        public string BaseDirectory => _directory;

        public string Name => _name;

        public string TodayDirectory
        {
            get { return _todayDirectory; }
            set { _todayDirectory = value; }
        }

        public log4net.Repository.Hierarchy.Logger DeviceLogger => _logger;

        public E_LOG_APPENDER IsAppender
        {
            get => _appenderType;
            set => _appenderType = value;
        }

        public bool IsOnlyFatalLog { get; set; } = false;

        public bool IsLogWithoutIntermediateDirectory { get; set; } = false;

        #endregion // Properties

        #region ▶  Constructor              ◀

        public LogDevice(
            string name
            , string dir
            , ref log4net.Repository.Hierarchy.Logger logger
            , ILogDevice parnetLogger = null)
        {
            if (dir.Contains(@":\"))
                _directory = dir;
            else
                _directory = $@"{System.IO.Directory.GetCurrentDirectory()}\{dir}";
            _name = name;
            _logger = logger;
            _log = new log4net.Core.LogImpl(_logger);
            _parentLogger = parnetLogger;
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
            _logger?.RemoveAllAppenders();
        }

        #endregion // Constructor

        #region ▶  Override                 ◀

        #region ▶  Override : Predicate	    ◀



        #endregion // Override : Predicate

        #region ▶  Override : Event Handler ◀

        public void Log_Debug(string strMessage)
        {
            if (_log.IsDebugEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                _log.Debug($"[{Name}] {strMessage}");

                _parentLogger?.Log_Debug($"[{Name}] {strMessage}");
                DebugLogHandle?.Invoke(strMessage);
            }
        }

        public void Log_Info(string strMessage)
        {
            if (_log.IsInfoEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                _log.Info($"[{Name}] {strMessage}");

                _parentLogger?.Log_Info($"[{Name}] {strMessage}");
                InfoLogHandle?.Invoke(strMessage);
            }
        }

        public void Log_Warn(string strMessage)
        {
            if (_log.IsWarnEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                _log.Warn($"[{Name}] {strMessage}");

                _parentLogger?.Log_Warn($"[{Name}] {strMessage}");
                WarnLogHandle?.Invoke(strMessage);
            }
        }

        public void Log_Error(string strMessage)
        {
            if (_log.IsErrorEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                _log.Error($"[{Name}] {strMessage}");

                _parentLogger?.Log_Error($"[{Name}] {strMessage}");
                ErrorLogHandle?.Invoke(strMessage);
            }
        }

        public void Log_Fatal(string strMessage)
        {
            if (_log.IsFatalEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                _log.Fatal($"[{Name}] {strMessage}");

                _parentLogger?.Log_Fatal($"[{Name}] {strMessage}");
                FatalLogHandle?.Invoke(strMessage);
            }
        }

        public void Log_Fatal(Exception ex)
        {
            if (_log.IsFatalEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                string strMessage = "";
                if (ex is UserException)
                    strMessage = $"{(ex as UserException).UserDetailMessage}\n[{ex.Source}] [{ex?.GetType()?.FullName}]\n{ex.StackTrace}";
                else
                    strMessage = $"{ex.Message}\n[{ex.Source}] [{ex?.GetType()?.FullName}]\n{ex.StackTrace}";
                _log.Fatal($"[{Name}] {strMessage}");

                _parentLogger?.Log_Fatal($"[{Name}] {strMessage}");
                FatalExcLogHandle?.Invoke(ex);
            }
        }

        public void Log_Fatal(string strMessage, Exception ex)
        {
            if (_log.IsFatalEnabled)
            {
                LogTracker.CheckLogAppenderDateUpdate(this);
                string strExtMessage = "";
                if (string.IsNullOrEmpty(strMessage) || string.IsNullOrWhiteSpace(strMessage))
                {
                    if (ex is UserException)
                        strExtMessage = $"{(ex as UserException).UserDetailMessage}\n[{ex.Source}] [{ex?.GetType()?.FullName}]\n{ex.StackTrace}";
                    else
                        strExtMessage = $"{ex.Message}\n[{ex.Source}] [{ex?.GetType()?.FullName}]\n{ex.StackTrace}";
                }
                else
                    strExtMessage = $"{strMessage}\n[{ex.Source}] [{ex?.GetType()?.FullName}]\n{ex.StackTrace}";
                _log.Fatal($"[{Name}] {strExtMessage}");

                _parentLogger?.Log_Fatal($"[{Name}] {strExtMessage}");
                FatalExtLogHandle?.Invoke(strMessage, ex);
            }
        }
        
        #endregion // Override : Event Handler

        #region ▶  Override : Function      ◀



        #endregion // Override : Function

        #endregion // Override

        #region ▶  Method                   ◀

        #region ▶  Method : Predicate	    ◀



        #endregion // Method : Predicate

        #region ▶  Method : Event Handler   ◀



        #endregion // Method : Event Handler

        #region ▶  Method : Function	    ◀



        #endregion // Method : Function

        #endregion // Method


        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Accessor

        #endregion

        #region Mutator

        #endregion

        #region Predicate

        #endregion
    }
}
