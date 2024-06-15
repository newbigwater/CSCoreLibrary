using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _000.Common._01._Definition;
using _000.Common._05._Class;
using _100.Logger;
using _100.Logger._01._Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _999.UnitTest._100._Logger
{
    [TestClass]
    public partial class _100_Logger
    {
        [TestCategory("[CATEGORY] 100_Logger")]
        [TestMethod("[100_Logger] Global Logger")]
        public void UnitTest_Logger_Global()
        {
            LogTracker.Initialize();
            try
            {
                LogTracker.Log_Debug("Debug Log 입니다.");
                LogTracker.Log_Info("Info Log 입니다.");
                LogTracker.Log_Warn("Warn Log 입니다.");
                LogTracker.Log_Error("Error Log 입니다.");

                var e = new UserException("Fatal Test", E_ERROR_CODE.INTERNAL_ERROR);
                LogTracker.Log_Fatal("Fatal1 Log 입니다.");
                LogTracker.Log_Fatal(e);
                LogTracker.Log_Fatal("Fatal3 Log 입니다.", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestCategory("[CATEGORY] 100_Logger")]
        [TestMethod("[100_Logger] [Global Logger] Top Logger")]
        public void UnitTest_Logger_Top()
        {
            UnitTest_MainMachine test = new UnitTest_MainMachine();
            try
            {
                test.Log_Debug("Debug Log 입니다.");
                test.Log_Info("Info Log 입니다.");
                test.Log_Warn("Warn Log 입니다.");
                test.Log_Error("Error Log 입니다.");


                var e = new UserException("Fatal Test", E_ERROR_CODE.INTERNAL_ERROR);
                test.Log_Fatal("Fatal1 Log 입니다.");
                test.Log_Fatal(e);
                test.Log_Fatal("Fatal3 Log 입니다.", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestCategory("[CATEGORY] 100_Logger")]
        [TestMethod("[100_Logger] [Global Logger] [Top Logger] Middle Logger")]
        public void UnitTest_Logger_Middle()
        {
            UnitTest_MainMachine test = new UnitTest_MainMachine();

            UnitTest_MainMachine_Sub _sub01 = null;
            UnitTest_MainMachine_Sub _sub02 = null;
            try
            {
                _sub01 = new UnitTest_MainMachine_Sub("Sub01", test);
                _sub01.Log_Debug("Debug Log 입니다.");
                _sub01.Log_Info("Info Log 입니다.");
                _sub01.Log_Warn("Warn Log 입니다.");
                _sub01.Log_Error("Error Log 입니다.");

                _sub02 = new UnitTest_MainMachine_Sub("Sub02", test);
                _sub02.Log_Debug("Debug Log 입니다.");
                _sub02.Log_Info("Info Log 입니다.");
                _sub02.Log_Warn("Warn Log 입니다.");
                _sub02.Log_Error("Error Log 입니다.");

                var e = new UserException("Fatal Test", E_ERROR_CODE.INTERNAL_ERROR);
                _sub01.Log_Fatal("Fatal1 Log 입니다.");
                _sub01.Log_Fatal(e);
                _sub01.Log_Fatal("Fatal3 Log 입니다.", e);

                _sub02.Log_Fatal("Fatal1 Log 입니다.");
                _sub02.Log_Fatal(e);
                _sub02.Log_Fatal("Fatal3 Log 입니다.", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestCategory("[CATEGORY] 100_Logger")]
        [TestMethod("[100_Logger] [Global Logger] [Top Logger] [Middle Logger] Low Logger")]
        public void UnitTest_Logger_Low()
        {
            UnitTest_MainMachine test = new UnitTest_MainMachine();

            UnitTest_MainMachine_Sub _sub01 = new UnitTest_MainMachine_Sub("Sub01", test);
            UnitTest_MainMachine_Sub _sub02 = new UnitTest_MainMachine_Sub("Sub02", test);


            UnitTest_MainMachine_Sub _sub01_unit01 = null;
            UnitTest_MainMachine_Sub _sub01_unit02 = null;
            UnitTest_MainMachine_Sub _sub02_unit01 = null;
            try
            {
                _sub01_unit01 = new UnitTest_MainMachine_Sub("Unit01", _sub01);
                _sub01_unit01.Log_Debug("Debug Log 입니다.");
                _sub01_unit01.Log_Info("Info Log 입니다.");
                _sub01_unit01.Log_Warn("Warn Log 입니다.");
                _sub01_unit01.Log_Error("Error Log 입니다.");

                _sub01_unit02 = new UnitTest_MainMachine_Sub("Unit02", _sub01);
                _sub01_unit02.Log_Debug("Debug Log 입니다.");
                _sub01_unit02.Log_Info("Info Log 입니다.");
                _sub01_unit02.Log_Warn("Warn Log 입니다.");
                _sub01_unit02.Log_Error("Error Log 입니다.");

                _sub02_unit01 = new UnitTest_MainMachine_Sub("Unit01", _sub02);
                _sub02_unit01.Log_Debug("Debug Log 입니다.");
                _sub02_unit01.Log_Info("Info Log 입니다.");
                _sub02_unit01.Log_Warn("Warn Log 입니다.");
                _sub02_unit01.Log_Error("Error Log 입니다.");

                var e = new UserException("Fatal Test", E_ERROR_CODE.INTERNAL_ERROR);
                _sub01_unit01.Log_Fatal("Fatal1 Log 입니다.");
                _sub01_unit01.Log_Fatal(e);
                _sub01_unit01.Log_Fatal("Fatal3 Log 입니다.", e);

                _sub01_unit02.Log_Fatal("Fatal1 Log 입니다.");
                _sub01_unit02.Log_Fatal(e);
                _sub01_unit02.Log_Fatal("Fatal3 Log 입니다.", e);

                _sub02_unit01.Log_Fatal("Fatal1 Log 입니다.");
                _sub02_unit01.Log_Fatal(e);
                _sub02_unit01.Log_Fatal("Fatal3 Log 입니다.", e);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public class UnitTest_MainMachine_Sub : ILogDevice
        {
            #region ▶  Event Handle             ◀

            #endregion // Event Handle

            #region ▶  Fields                   ◀

            private LogDevice _logger = null;

            #endregion // Fields

            #region ▶  Properties               ◀



            #endregion // Properties

            #region ▶  Constructor              ◀

            public UnitTest_MainMachine_Sub(string name, ILogDevice parentLogger = null)
            {
                _logger = LogTracker.GetLogDevice(name, parentLogger);
            }

            #endregion // Constructor

            #region ▶  Override                 ◀

            #region ▶  Override : Predicate	    ◀



            #endregion // Override : Predicate

            #region ▶  Override : Event Handler ◀

            public void Log_Debug(string strMessage)
            {
                _logger.Log_Debug(strMessage);
            }

            public void Log_Info(string strMessage)
            {
                _logger.Log_Info(strMessage);
            }

            public void Log_Warn(string strMessage)
            {
                _logger.Log_Warn(strMessage);
            }

            public void Log_Error(string strMessage)
            {
                _logger.Log_Error(strMessage);
            }

            public void Log_Fatal(string strMessage)
            {
                _logger.Log_Debug(strMessage);
            }

            public void Log_Fatal(Exception ex)
            {
                _logger.Log_Fatal(ex);
            }

            public void Log_Fatal(string strMessage, Exception ex)
            {
                _logger.Log_Fatal(strMessage, ex);
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
        }

        public class UnitTest_MainMachine : ILogDevice
        {
            #region ▶  Event Handle             ◀

            #endregion // Event Handle

            #region ▶  Fields                   ◀

            private LogDevice _logger = null;


            #endregion // Fields

            #region ▶  Properties               ◀



            #endregion // Properties

            #region ▶  Constructor              ◀

            public UnitTest_MainMachine(ILogDevice parentLogger = null)
            {
                _logger = LogTracker.GetLogDevice("MainMachine");
            }

            #endregion // Constructor

            #region ▶  Override                 ◀

            #region ▶  Override : Predicate	    ◀



            #endregion // Override : Predicate

            #region ▶  Override : Event Handler ◀

            public void Log_Debug(string strMessage)
            {
                _logger.Log_Debug(strMessage);
            }

            public void Log_Info(string strMessage)
            {
                _logger.Log_Info(strMessage);
            }

            public void Log_Warn(string strMessage)
            {
                _logger.Log_Warn(strMessage);
            }

            public void Log_Error(string strMessage)
            {
                _logger.Log_Error(strMessage);
            }

            public void Log_Fatal(string strMessage)
            {
                _logger.Log_Debug(strMessage);
            }

            public void Log_Fatal(Exception ex)
            {
                _logger.Log_Fatal(ex);
            }

            public void Log_Fatal(string strMessage, Exception ex)
            {
                _logger.Log_Fatal(strMessage, ex);
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
        }
    }
}
