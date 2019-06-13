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
using System.Collections.ObjectModel;
using System.Windows.Input;
using Cast.Util.Log;
using CastReporting.BLL;
using CastReporting.Domain;

namespace CastReporting.UI.WPF.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectWSVM : ViewModelBase
    {
       
        /// <summary>
        /// 
        /// </summary>
        public ICommand AddCommand { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveCommand { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ICommand ActiveCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _newConnectionUrl;
        public string NewConnectionUrl
        {
            get
            {
                return _newConnectionUrl;
            }
            set
            {
                _newConnectionUrl = value;

                OnPropertyChanged("NewConnectionUrl");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string _newConnectionPassword;
        public string NewConnectionPassword
        {
            get
            {
                return _newConnectionPassword;
            }
            set
            {
                _newConnectionPassword = value;

                OnPropertyChanged("NewConnectionPassword");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string _newConnectionLogin;
        public string NewConnectionLogin
        {
            get
            {
                return _newConnectionLogin;
            }
            set
            {
                _newConnectionLogin = value;

                OnPropertyChanged("NewConnectionLogin");
            }

        }

        /// <summary>
        /// 
        /// </summary>       
        private ObservableCollection<WSConnection> _wsConnections;
        public ObservableCollection<WSConnection> WSConnections 
        { 
            get
            {
                return _wsConnections;
            } 
            set
            {
                _wsConnections = value;

                 OnPropertyChanged("WSConnections");
            } 
            
        }

        /// <summary>
        /// 
        /// </summary>
        private WSConnection _selectedWSConnection;
        public WSConnection SelectedWSConnection
        {
            get
            {
                return _selectedWSConnection;
            }
            set
            {
                _selectedWSConnection = value;

                OnPropertyChanged("SelectedWSConnection");
            }

        }

        /// <summary>
        ///
        /// </summary>
        public SelectWSVM()
        {

            AddCommand = new CommandHandler(ExecuteAddCommand, null);

            RemoveCommand = new CommandHandler(ExecuteRemoveCommand, null);

            ActiveCommand = new CommandHandler(ExecuteActiveCommand, null);

            WSConnections = new ObservableCollection<WSConnection>(Setting.WSConnections);
           
        }

        /// <summary>
        /// Implement Add service Command
        /// </summary>
        private void ExecuteAddCommand(object prameter)
        {
            try
            {

                WSConnection conn = new WSConnection(NewConnectionUrl, NewConnectionLogin, NewConnectionPassword, string.Empty);

                StatesEnum state;
                Setting = SettingsBLL.AddConnection(conn, false, out state);

                if (state == StatesEnum.ConnectionAddedAndActivated || state == StatesEnum.ConnectionAddedSuccessfully)
                {
                    WSConnections = new ObservableCollection<WSConnection>(Setting.WSConnections);

                    NewConnectionUrl = NewConnectionLogin = NewConnectionPassword = string.Empty;
                }

                MessageManager.OnServiceAdded(conn.Url, state);
            }
            catch (UriFormatException ex)
            {
                LogHelper.Instance.LogInfo(ex.Message);
                MessageManager.OnServiceAdded(NewConnectionUrl, StatesEnum.ServiceInvalid);
            }
        }

        /// <summary>
        /// Implement remove service Command
        /// </summary>
        private void ExecuteRemoveCommand(object prameter)
        {
            if (SelectedWSConnection == null) return;
            string tmpUrl = SelectedWSConnection.Url;

            Setting = SettingsBLL.RemoveConnection(SelectedWSConnection);
            WSConnections = new ObservableCollection<WSConnection>(Setting.WSConnections);

            MessageManager.OnServiceRemoved(tmpUrl);
        }


        /// <summary>
        /// Implement active service Command
        /// </summary>
        private void ExecuteActiveCommand(object prameter)
        {
            if (SelectedWSConnection == null) return;
            Setting.ChangeActiveConnection(SelectedWSConnection.Url);

            SettingsBLL.SaveSetting(Setting);

            MessageManager.OnServiceActivated(SelectedWSConnection.Url);

            WSConnections = new ObservableCollection<WSConnection>(Setting.WSConnections);
        }


    }
}
