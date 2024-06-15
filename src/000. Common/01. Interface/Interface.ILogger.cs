using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _100.Logger._01._Interface
{
    public delegate void LogDebugHandler(string strMessage);
    public delegate void LogInfoHandler(string strMessage);
    public delegate void LogWarnHandler(string strMessage);
    public delegate void LogErrorHandler(string strMessage);
    public delegate void LogFatalHandler(string strMessage);
    public delegate void LogFatalExcHandler(Exception ex);
    public delegate void LogFatalExtHandler(string strMessage, Exception ex);

    /// <summary>
    /// Log 출력을 위한 Interface
    /// </summary>
    public interface ILogDevice
    {
        /* DAO (Data Access Object) */
        #region ▶  Event Handle             ◀


        #endregion // Event Handle

        #region ▶  Constructor              ◀



        #endregion // Constructor

        #region ▶  Method                   ◀

        #region ▶  Method : Predicate	    ◀



        #endregion // Method : Predicate

        #region ▶  Method : Event Handler   ◀

        /// <summary>
        /// Debug 로그를 출력합니다.
        /// <br/> Normal Level에서는 출력되지 않습니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Debug(string strMessage);

        /// <summary>
        /// Information 로그를 출력합니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Info(string strMessage);

        /// <summary>
        /// Debug 로그를 출력합니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Warn(string strMessage);

        /// <summary>
        /// Debug 로그를 출력합니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Error(string strMessage);

        /// <summary>
        /// Debug 로그를 출력합니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Fatal(string strMessage);

        /// <summary>
        /// Debug 로그를 출력합니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Fatal(Exception ex);

        /// <summary>
        /// Debug 로그를 출력합니다.
        /// </summary>
        /// <param name="strMessage">Debug 로그 문자열을 입력합니다.</param>
        /// <returns></returns>
        void Log_Fatal(string strMessage, Exception ex);

        #endregion // Method : Event Handler

        #region ▶  Method : Function	    ◀



        #endregion // Method : Function

        #endregion // Method
    }
}
