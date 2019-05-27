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

using System.Collections.Generic;
using System.Windows.Controls;
using CastReporting.Domain;
using CastReporting.UI.WPF.ViewModel;

namespace CastReporting.UI.WPF.View
{
    /// <summary>
    /// Interaction logic for SelectWS
    /// </summary>
    public partial class SelectWS : Page
    {
        
        /// <summary>
        /// 
        /// </summary>
        public SelectWS()
        {
            InitializeComponent();

            DataContext = new SelectWSVM();
             
        }

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

                (DataContext as SelectWSVM)?.ExecuteAddCommand(connection);
            }
            e.Handled = true;
        }
    }

}

