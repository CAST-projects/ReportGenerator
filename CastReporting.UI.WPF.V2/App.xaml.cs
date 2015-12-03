
/*
 *   Copyright (c) 2015 CAST
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
using Cast.Util.Log;
using CastReporting.BLL;
using CastReporting.UI.WPF.Resources.Languages;
using CastReporting.UI.WPF.ViewModel;
using System;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;



namespace CastReporting.UI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        public App()
        {
#if !DEBUG
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;         
#endif            
            LogHelper.SetPathLog(SettingsBLL.GetApplicationPath());

            if (!String.IsNullOrEmpty( ViewModelBase.Setting.ReportingParameter.CultureName))
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(ViewModelBase.Setting.ReportingParameter.CultureName);

                if(cultureInfo != null)
                {
                    Thread.CurrentThread.CurrentCulture = cultureInfo;
                    Thread.CurrentThread.CurrentUICulture =cultureInfo;                  
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception currentException;
            
            if (e.Exception.InnerException is WebException)
                currentException = e.Exception.InnerException;
            if (e.Exception.InnerException is AggregateException)
                currentException = (e.Exception.InnerException as AggregateException).GetBaseException();
            else
                currentException = e.Exception;                            
                   
            LogHelper.Instance.LogError(Messages.msgGenericError, currentException);
            
            IMessageManager messageManager = (this.MainWindow.DataContext as IMessageManager);
            messageManager.OnErrorOccured(currentException);

            messageManager.SetBusyMode(false);

            e.Handled = true;
        }
    }
}
