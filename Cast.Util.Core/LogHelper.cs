/*
 *   Copyright (c) 2019 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System.IO;
using System.Reflection;
using log4net.Config;

namespace Cast.Util.Log
{
    public class LogHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private static volatile ILog _log;

        /// <summary>
        /// 
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// 
        /// </summary>
        private static LogHelper _instance;
        public static LogHelper Instance
        {
            get
            {
                if (null == _instance)
                {
                    lock (Lock)
                    {
                        if (null == _instance)
                        {
                            _instance = new LogHelper();
                        }
                    }
                }
                return _instance;
            }
        }

      

        /// <summary>
        /// 
        /// </summary>
        private LogHelper()
        {
            if (_log == null)
                _log = LogManager.GetLogger(typeof(LogHelper));           
        }

        #region METHODS - Log4Net

        public void LogDebug(string message)
        {
            _log.Debug(message);
        }
        public void LogDebug(string message, Exception exception)
        {
            _log.Debug(message, exception);
        }
        public void LogDebugFormat(string messageFormat, params object[] parameters)
        {
            _log.DebugFormat(messageFormat, parameters);
        }
        public void LogDebugFormat(string messageFormat, object obj)
        {
            _log.DebugFormat(messageFormat, obj);
        }
        public void LogDebugFormat(string messageFormat, object obj1, object obj2)
        {
            _log.DebugFormat(messageFormat, obj1, obj2);
        }
        public void LogDebugFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            _log.DebugFormat(messageFormat, obj1, obj2, obj3);
        }

        public void LogInfo(string message)
        {
            _log.Info(message);
        }
        public void LogInfo(string message, Exception exception)
        {
            _log.Info(message, exception);
        }
        public void LogInfoFormat(string messageFormat, params object[] parameters)
        {
            _log.InfoFormat(messageFormat, parameters);
        }
        public void LogInfoFormat(string messageFormat, object obj)
        {
            _log.InfoFormat(messageFormat, obj);
        }
        public void LogInfoFormat(string messageFormat, object obj1, object obj2)
        {
            _log.InfoFormat(messageFormat, obj1, obj2);
        }
        public void LogInfoFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            _log.InfoFormat(messageFormat, obj1, obj2, obj3);
        }

        public void LogWarn(string message)
        {
            _log.Warn(message);
        }
        public void LogWarn(string message, Exception exception)
        {
            _log.Warn(message, exception);
        }
        public void LogWarnFormat(string messageFormat, params object[] parameters)
        {
            _log.WarnFormat(messageFormat, parameters);
        }
        public void LogWarnFormat(string messageFormat, object obj)
        {
            _log.WarnFormat(messageFormat, obj);
        }
        public void LogWarnFormat(string messageFormat, object obj1, object obj2)
        {
            _log.WarnFormat(messageFormat, obj1, obj2);
        }
        public void LogWarnFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            _log.WarnFormat(messageFormat, obj1, obj2, obj3);
        }

        public void LogError(string message)
        {
            _log.Error(message);
        }
        public void LogError(string message, Exception exception)
        {
            _log.Error(message, exception);
        }
        public void LogErrorFormat(string messageFormat, params object[] parameters)
        {
            _log.ErrorFormat(messageFormat, parameters);
        }
        public void LogErrorFormat(string messageFormat, object obj)
        {
            _log.ErrorFormat(messageFormat, obj);
        }
        public void LogErrorFormat(string messageFormat, object obj1, object obj2)
        {
            _log.ErrorFormat(messageFormat, obj1, obj2);
        }
        public void LogErrorFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            _log.ErrorFormat(messageFormat, obj1, obj2, obj3);
        }

        public void LogFatal(string message)
        {
            _log.Fatal(message);
        }
        public void LogFatal(string message, Exception exception)
        {
            _log.Fatal(message, exception);
        }
        public void LogFatalFormat(string messageFormat, params object[] parameters)
        {
            _log.FatalFormat(messageFormat, parameters);
        }
        public void LogFatalFormat(string messageFormat, object obj)
        {
            _log.FatalFormat(messageFormat, obj);
        }
        public void LogFatalFormat(string messageFormat, object obj1, object obj2)
        {
            _log.FatalFormat(messageFormat, obj1, obj2);
        }
        public void LogFatalFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            _log.FatalFormat(messageFormat, obj1, obj2, obj3);
        }

        public void FlushLog()
        {
            var logger = _log.Logger as Logger;
            if (logger == null) return;
            foreach (IAppender appender in logger.Appenders)
            {
                var buffered = appender as BufferingAppenderSkeleton;
                buffered?.Flush();
            }
        }
        #endregion METHODS - Log4Net

        #region Private METHODS 
        /// <summary>
        /// Set Template Path Registry
        /// </summary>
        public static void  SetPathLog(string pathLog)
        {
            GlobalContext.Properties["APPNAME"] = "ReportGenerator";
            GlobalContext.Properties["LOGPATH"] = pathLog;
            GlobalContext.Properties["DATE"] = DateTime.Today.ToString("yyyyMMdd");
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            string configFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "log4net.config");
            //Console.Out.WriteLine(configFilePath);
            XmlConfigurator.Configure(logRepository, new FileInfo(configFilePath));
        }

        #endregion
    }
}
