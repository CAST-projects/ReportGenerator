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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
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
        public ICommand TestCommand { get; set; }

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
        private string _NewConnectionUrl;
        public string NewConnectionUrl
        {
            get
            {
                return _NewConnectionUrl;
            }
            set
            {
                _NewConnectionUrl = value;

                base.OnPropertyChanged("NewConnectionUrl");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string _NewConnectionPassword;
        public string NewConnectionPassword
        {
            get
            {
                return _NewConnectionPassword;
            }
            set
            {
                _NewConnectionPassword = value;

                base.OnPropertyChanged("NewConnectionPassword");
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private string _NewConnectionLogin;
        public string NewConnectionLogin
        {
            get
            {
                return _NewConnectionLogin;
            }
            set
            {
                _NewConnectionLogin = value;

                base.OnPropertyChanged("NewConnectionLogin");
            }

        }

        /// <summary>
        /// 
        /// </summary>       
        private ObservableCollection<WSConnection> _WSConnections;
        public ObservableCollection<WSConnection> WSConnections 
        { 
            get
            {
                return _WSConnections;
            } 
            set
            {
                _WSConnections = value;

                 base.OnPropertyChanged("WSConnections");
            } 
            
        }

        /// <summary>
        /// 
        /// </summary>
        private WSConnection _SelectedWSConnection;
        public WSConnection SelectedWSConnection
        {
            get
            {
                return _SelectedWSConnection;
            }
            set
            {
                _SelectedWSConnection = value;

                base.OnPropertyChanged("SelectedWSConnection");
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

            TestCommand = new CommandHandler(ExecuteTestCommand, null);

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

                base.MessageManager.OnServiceAdded(conn.Url, state);
            }
            catch (UriFormatException ex)
            {
                base.MessageManager.OnServiceAdded(NewConnectionUrl, StatesEnum.ServiceInvalid);
            }
        }

        /// <summary>
        /// Implement remove service Command
        /// </summary>
        private void ExecuteRemoveCommand(object prameter)
        {
            if (SelectedWSConnection != null)
            {
                string tmpUrl = SelectedWSConnection.Url;

                Setting = SettingsBLL.RemoveConnection(SelectedWSConnection);
                WSConnections = new ObservableCollection<WSConnection>(Setting.WSConnections);

                base.MessageManager.OnServiceRemoved(tmpUrl);

               
            }
        }


        /// <summary>
        /// Implement active service Command
        /// </summary>
        private void ExecuteActiveCommand(object prameter)
        {
            if (SelectedWSConnection != null)
            {               
                Setting.ChangeActiveConnection(SelectedWSConnection.Url);

                SettingsBLL.SaveSetting(Setting);

                base.MessageManager.OnServiceActivated(SelectedWSConnection.Url);

                WSConnections = new ObservableCollection<WSConnection>(Setting.WSConnections);               
            }
        }

        /// <summary>
        /// Implement test service Command
        /// </summary>
        private void ExecuteTestCommand(object prameter)
        {
            bool resutTest = true;

            if (prameter == null) //Test of WS defined on "Add service area" 
            {
                if (!String.IsNullOrEmpty(NewConnectionUrl) && Uri.IsWellFormedUriString(NewConnectionUrl, UriKind.Absolute))
                {
                    using (CommonBLL commonBLL = new CommonBLL(new WSConnection(NewConnectionUrl, NewConnectionLogin, NewConnectionPassword, string.Empty))) 
                    {
                       resutTest=  commonBLL.CheckService();

                       base.MessageManager.OnServiceChecked(NewConnectionUrl, resutTest);
                    }
                }
            }
            else
            {
                using (CommonBLL commonBLL = new CommonBLL(SelectedWSConnection)) 
                {
                    resutTest = commonBLL.CheckService();

                    base.MessageManager.OnServiceChecked(SelectedWSConnection.Url, resutTest);
                }
            }            
        }

       

    }
}
