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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CastReporting.UI.WPF.ViewModel;
using Microsoft.Win32;
using CastReporting.Domain;
using System.Collections.Generic;
using System.IO;
using System;
using CastReporting.BLL;

namespace CastReporting.UI.WPF.View
{
    /// <summary>
    /// Interaction logic for Reporting1.xaml
    /// </summary>
    public partial class Reporting : Page
    {
        public Reporting()
        {
            InitializeComponent();

            DataContext = new ReportingVM();

            Loaded += OnLoaded; 

        }

        public WSConnection ActiveConnection { get; set; }

        public new System.Windows.Input.CommandBindingCollection CommandBindings { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as ReportingVM)?.InitializeFromWS();
        }



        /// <summary>
        /// Show Selection File Dialog 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButtonClicked(object sender, RoutedEventArgs e)
        {
            ReportingVM _reportingVm = DataContext as ReportingVM;

            if (_reportingVm == null) return;
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = _reportingVm.SelectedTemplateFile.Extension != ".xlsx" ?
                    string.Format("*{0}, *.pdf|*{0};*.pdf", _reportingVm.SelectedTemplateFile.Extension)
                    : string.Format("*{0}|*{0}", _reportingVm.SelectedTemplateFile.Extension),
                DefaultExt = _reportingVm.SelectedTemplateFile.Extension,
                FileName = _reportingVm.SelectedTemplateFile.Name.Replace('-',' ')
            };


            var settings = SettingsBLL.GetSetting();

            if (string.IsNullOrEmpty(settings.ReportingParameter.TemplatePath) || !Directory.Exists(settings.ReportingParameter.TemplatePath))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            else
            {
                dialog.InitialDirectory = settings.ReportingParameter.GeneratedFilePath;
            }

            var result = dialog.ShowDialog();
            var _vm = (ReportingVM) DataContext;
            if (result != null && result.Value)
            {
                settings.ReportingParameter.GeneratedFilePath = Path.GetDirectoryName(dialog.FileName);

                SettingsBLL.SaveSetting(settings);

                if (_vm != null) _vm.ReportFileName = dialog.FileName;
            }
            else
            {
                
                if (_vm != null) _vm.ReportFileName = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileListDoubleClicked(object sender, RoutedEventArgs e)
        {
            var _selectedTemplateFile = (DataContext as ReportingVM)?.SelectedTemplateFile;
            if (_selectedTemplateFile != null) Process.Start(_selectedTemplateFile.FullName);
        }

        private void ActivateWebService_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivateWebService_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var list = e.Parameter as List<object>;
            if (list != null)
            {
                var connection = new WSConnection
                {
                    Url = (string)list[0],
                    Login = (string)list[1],
                    Password = (string)list[2],
                    ApiKey = (bool)list[3]
                };

                (DataContext as ReportingVM)?.ActiveCurrentWebService(connection);
            }
            (DataContext as ReportingVM)?.InitializeFromWS();
            e.Handled = true;
        }
    }
}