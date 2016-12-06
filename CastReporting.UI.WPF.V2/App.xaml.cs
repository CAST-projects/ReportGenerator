
/*
 *   Copyright (c) 2016 CAST
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
    // ReSharper disable once RedundantExtendsListEntry
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

            if (string.IsNullOrEmpty(ViewModelBase.Setting.ReportingParameter.CultureName)) return;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(ViewModelBase.Setting.ReportingParameter.CultureName);

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture =cultureInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception currentException;
            
            if (e.Exception.InnerException is WebException)
            { 
                // ReSharper disable once RedundantAssignment
                currentException = e.Exception.InnerException;
            }
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (e.Exception.InnerException is AggregateException)
                currentException = ((AggregateException) e.Exception.InnerException).GetBaseException();
            else
                currentException = e.Exception;                            
                   
            LogHelper.Instance.LogError(Messages.msgGenericError, currentException);
            
            IMessageManager messageManager = (MainWindow.DataContext as IMessageManager);
            messageManager?.OnErrorOccured(currentException);

            messageManager?.SetBusyMode(false);

            e.Handled = true;
        }
    }
}
