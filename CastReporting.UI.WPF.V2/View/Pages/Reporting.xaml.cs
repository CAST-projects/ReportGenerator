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
using CastReporting.Domain;
using System.Collections.Generic;
using System.IO;
using System;
using System.Security;
using Cast.Util.Log;
using CastReporting.BLL;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TreeView = System.Windows.Controls.TreeView;

namespace CastReporting.UI.WPF.View
{
    /// <summary>
    /// Interaction logic for Reporting1.xaml
    /// </summary>
    public partial class Reporting : Page
    {
        private static readonly List<string> ExtensionList = new List<string> {"xlsx", "docx", "pptx"};

        public Reporting()
        {
            InitializeComponent();
            DataContext = new ReportingVM();
            Loaded += OnLoaded;
            LoadTemplates();
        }

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if (item?.Items.Count != 1 || !(item.Items[0] is string)) return;
            item.Items.Clear();

            DirectoryInfo _dir = item.Tag as DirectoryInfo;
            if (_dir == null) return;
            try
            {
                ListDirectory((TreeView)sender, _dir.FullName);
            }
            catch (Exception ex) when (ex is DirectoryNotFoundException || ex is SecurityException || ex is UnauthorizedAccessException || ex is InvalidOperationException)
            {
                LogHelper.LogError("Cannot expand folders : " + ex.Message);
            }
        }

        private static void ListDirectory(TreeView treeView, string path)
        {
            treeView.Items.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            foreach (var directory in rootDirectoryInfo.GetDirectories())
                treeView.Items.Add(CreateDirectoryNode(directory));
            foreach (var file in rootDirectoryInfo.GetFiles())
            {
                if (ExtensionList.Contains(file.Extension))
                {
                    treeView.Items.Add(new TreeViewItem { Header = file.Name, Tag = file.FullName });
                }
            }
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name, Tag = directoryInfo.FullName };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Items.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name, Tag = file.FullName});

            return directoryNode;
        }


        private void ReloadTemplatesClicked(object sender, RoutedEventArgs e)
        {
            LoadTemplates();
        }

        private void LoadTemplates()
        {
            ReportingVM _reportingVm = (ReportingVM) DataContext;
            switch (_reportingVm.SelectedTab)
            {
                case 0:
                    ListDirectory(TrvStructure, SettingsBLL.GetApplicationTemplateRootPath());
                    break;
                case 1:
                    ListDirectory(TrvStructure, SettingsBLL.GetPortfolioTemplateRootPath());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            TreeViewItem selectedTreeViewItem = (TreeViewItem)TrvStructure.SelectedItem;
            FileInfo selectedFileInfo = new FileInfo(selectedTreeViewItem.Tag.ToString());
            if (selectedFileInfo.Exists)
            {
                Process.Start(selectedFileInfo.FullName);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedTreeViewItem = (TreeViewItem)TrvStructure.SelectedItem;
            if (selectedTreeViewItem == null) return;
            FileInfo selectedFileInfo = new FileInfo(selectedTreeViewItem.Tag.ToString());
            if (selectedFileInfo.Exists)
            {
                ((ReportingVM) DataContext).SelectedTemplateFile = selectedFileInfo;
            }
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
                    ApiKey = (bool)list[3],
                    ServerCertificateValidation = SettingsBLL.GetCertificateValidationStrategy()
                };

                (DataContext as ReportingVM)?.ActiveCurrentWebService(connection);
            }
            (DataContext as ReportingVM)?.InitializeFromWS();
            e.Handled = true;
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTemplates();
        }
    }
}