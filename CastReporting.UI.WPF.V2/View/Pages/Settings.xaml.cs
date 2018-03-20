/*
 *   Copyright (c) 2018 CAST
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
using CastReporting.UI.WPF.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace CastReporting.UI.WPF.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public bool HasError => !IsValid(this);

        private bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors, 
            //and all of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
                LogicalTreeHelper.GetChildren(obj)
                .OfType<DependencyObject>()
                .All(IsValid);
        }

        public Settings()
        {
            InitializeComponent();

            DataContext = new SettingsVM();
         
        }

        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler LangageChanged
        {
            add
            {
                var _settingsVm = DataContext as SettingsVM;
                if (_settingsVm != null) _settingsVm.LangageChanged += value;
            }
            remove
            {
                var _settingsVm = DataContext as SettingsVM;
                if (_settingsVm != null) _settingsVm.LangageChanged -= value;
            } 
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK) TxtTemplatePath.Text = dialog.SelectedPath;

  
        }

        private void OpenSaveSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !HasError;
            e.Handled = true;
        }

        private void OpenSaveSettings_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var _settingsVm = DataContext as SettingsVM;
            _settingsVm?.SaveSettings();
            e.Handled = true;
        }
    }
}
