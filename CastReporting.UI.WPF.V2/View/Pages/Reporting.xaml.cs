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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CastReporting.UI.WPF.Resources.Languages;
using CastReporting.UI.WPF.ViewModel;
using Microsoft.Win32;
using CastReporting.Domain;
using System.Collections.Generic;
using System.IO;
using System;
using CastReporting.BLL;
using System.Data;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages; 
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Xml;

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

            this.DataContext = new ReportingVM();

            this.Loaded += OnLoaded; 

            //BindCategories();
        }

        private WSConnection _ActiveConnection;
        public WSConnection ActiveConnection
        {
            get
            {
                return _ActiveConnection;
            }
            set
            {
                _ActiveConnection = value;
            }

        }
        
        private readonly System.Windows.Input.CommandBindingCollection _CommandBindings;
        public System.Windows.Input.CommandBindingCollection CommandBindings
        {
            get
            {
                return _CommandBindings;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ReportingVM).InitializeFromWS();
        }



        /// <summary>
        /// Show Selection File Dialog 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButtonClicked(object sender, RoutedEventArgs e)
        {
            ReportingVM reportingVM = (this.DataContext as ReportingVM);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = string.Format("*{0}|*{0}", reportingVM.SelectedTemplateFile.Extension);

            dialog.DefaultExt = reportingVM.SelectedTemplateFile.Extension;

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

            if (result.Value)
            {
                settings.ReportingParameter.GeneratedFilePath = Path.GetDirectoryName(dialog.FileName);

                SettingsBLL.SaveSetting(settings);

                (this.DataContext as ReportingVM).ReportFileName = dialog.FileName;
            }
            else
                (this.DataContext as ReportingVM).ReportFileName = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileListDoubleClicked(object sender, RoutedEventArgs e)
        {
            Process.Start((this.DataContext as ReportingVM).SelectedTemplateFile.FullName);
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
            var list = e.Parameter as List<string>;
            if (list != null)
            {
                var connection = new WSConnection
                {
                    Url = list[0],
                    Login = list[1],
                    Password = list[2]
                };

                (this.DataContext as ReportingVM).ActiveCurrentWebService(connection);
            }
            (this.DataContext as ReportingVM).InitializeFromWS();
            e.Handled = true;
        }

    }
}