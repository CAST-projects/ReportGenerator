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

namespace Cast.Util.Log
{
    public class LogHelper
    {
        private LogHelper()
        {
            // Avoid instanciation of the class
        }

        private static readonly Lazy<ILog> Log = new Lazy<ILog>(() => LogManager.GetLogger(typeof(LogHelper)));

        #region METHODS - Log4Net

        public static void LogDebug(string message)
        {
            Log.Value.Debug(message);
        }
        public static void LogDebug(string message, Exception exception)
        {
            Log.Value.Debug(message, exception);
        }
        public static void LogDebugFormat(string messageFormat, params object[] parameters)
        {
            Log.Value.DebugFormat(messageFormat, parameters);
        }
        public static void LogDebugFormat(string messageFormat, object obj)
        {
            Log.Value.DebugFormat(messageFormat, obj);
        }
        public static void LogDebugFormat(string messageFormat, object obj1, object obj2)
        {
            Log.Value.DebugFormat(messageFormat, obj1, obj2);
        }
        public static void LogDebugFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            Log.Value.DebugFormat(messageFormat, obj1, obj2, obj3);
        }

        public static void LogInfo(string message)
        {
            Log.Value.Info(message);
        }
        public static void LogInfo(string message, Exception exception)
        {
            Log.Value.Info(message, exception);
        }
        public static void LogInfoFormat(string messageFormat, params object[] parameters)
        {
            Log.Value.InfoFormat(messageFormat, parameters);
        }
        public static void LogInfoFormat(string messageFormat, object obj)
        {
            Log.Value.InfoFormat(messageFormat, obj);
        }
        public static void LogInfoFormat(string messageFormat, object obj1, object obj2)
        {
            Log.Value.InfoFormat(messageFormat, obj1, obj2);
        }
        public static void LogInfoFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            Log.Value.InfoFormat(messageFormat, obj1, obj2, obj3);
        }

        public static void LogWarn(string message)
        {
            Log.Value.Warn(message);
        }
        public static void LogWarn(string message, Exception exception)
        {
            Log.Value.Warn(message, exception);
        }
        public static void LogWarnFormat(string messageFormat, params object[] parameters)
        {
            Log.Value.WarnFormat(messageFormat, parameters);
        }
        public static void LogWarnFormat(string messageFormat, object obj)
        {
            Log.Value.WarnFormat(messageFormat, obj);
        }
        public static void LogWarnFormat(string messageFormat, object obj1, object obj2)
        {
            Log.Value.WarnFormat(messageFormat, obj1, obj2);
        }
        public static void LogWarnFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            Log.Value.WarnFormat(messageFormat, obj1, obj2, obj3);
        }

        public static void LogError(string message)
        {
            Log.Value.Error(message);
        }
        public static void LogError(string message, Exception exception)
        {
            Log.Value.Error(message, exception);
        }
        public static void LogErrorFormat(string messageFormat, params object[] parameters)
        {
            Log.Value.ErrorFormat(messageFormat, parameters);
        }
        public static void LogErrorFormat(string messageFormat, object obj)
        {
            Log.Value.ErrorFormat(messageFormat, obj);
        }
        public static void LogErrorFormat(string messageFormat, object obj1, object obj2)
        {
            Log.Value.ErrorFormat(messageFormat, obj1, obj2);
        }
        public static void LogErrorFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            Log.Value.ErrorFormat(messageFormat, obj1, obj2, obj3);
        }

        public static void LogFatal(string message)
        {
            Log.Value.Fatal(message);
        }
        public static void LogFatal(string message, Exception exception)
        {
            Log.Value.Fatal(message, exception);
        }
        public static void LogFatalFormat(string messageFormat, params object[] parameters)
        {
            Log.Value.FatalFormat(messageFormat, parameters);
        }
        public static void LogFatalFormat(string messageFormat, object obj)
        {
            Log.Value.FatalFormat(messageFormat, obj);
        }
        public static void LogFatalFormat(string messageFormat, object obj1, object obj2)
        {
            Log.Value.FatalFormat(messageFormat, obj1, obj2);
        }
        public static void LogFatalFormat(string messageFormat, object obj1, object obj2, object obj3)
        {
            Log.Value.FatalFormat(messageFormat, obj1, obj2, obj3);
        }

        public static void FlushLog()
        {
            var logger = Log.Value.Logger as Logger;
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
            log4net.Config.XmlConfigurator.Configure();
        }

        #endregion
    }
}
