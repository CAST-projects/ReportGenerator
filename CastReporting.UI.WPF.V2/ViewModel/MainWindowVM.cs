
using CastReporting.BLL;
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
using CastReporting.UI.WPF.Common;
using CastReporting.UI.WPF.Resources.Languages;
using System;
using System.Collections.ObjectModel;
using System.Net;

namespace CastReporting.UI.WPF.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class MainWindowVM : ViewModelBase, IMessageManager
    {
       
        /// <summary>
        /// 
        /// </summary>
        private double _progressPercentage;
        public double ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                _progressPercentage = value;
                OnPropertyChanged("ProgressPercentage");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _progressMessage;
        public string ProgressMessage
        {
            get { return _progressMessage; }
            set
            {
                _progressMessage = value;
                OnPropertyChanged("ProgressMessage");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        

        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<MessageItem> _messagesList = new ObservableCollection<MessageItem>();
        public ObservableCollection<MessageItem> MessagesList
        {
            get { return _messagesList; }
            set
            {
                if (value == _messagesList)
                    return;
                
                _messagesList = value;
                
                OnPropertyChanged("MessagesList");
            }
        }

        #region IMessageManager implementation

        /// <summary>
        /// 
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                if (!_isBusy)
                {
                    ProgressPercentage = 0;
                }
                OnPropertyChanged("IsBusy");
            }
        }


        /// <summary>
        /// Handle the message display after report generated 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="timeSpan"></param>
        public void OnReportGenerated(string fileName, TimeSpan timeSpan)
        {
            MessagesList.Add(new MessageItem { Message = Messages.msgReportGenerationSuccess, FileName = fileName });

#if DEBUG
            MessagesList.Add(new MessageItem { Message = $"Generation duration : {timeSpan}"}); 
#endif
        }

        /// <summary>
        /// Handle the message display after service added
        /// </summary>
        /// <param name="message"></param>
        /// <param name="state"></param>
        public void OnServiceAdded(string message, StatesEnum state)
        {
            switch (state)
            {
                case StatesEnum.ServiceInvalid:
                    MessagesList.Add(new MessageItem { Message = Messages.msgServiceAddedFailed, FileName = string.Empty });
                    break;

                case StatesEnum.ConnectionAlreadyExist:
                    MessagesList.Add(new MessageItem { Message = Messages.msgServiceAddedAlreadyExist, FileName = string.Empty });
                    break;

                case StatesEnum.ConnectionAddedAndActivated:
                    MessagesList.Add(new MessageItem { Message = Messages.msgServiceActivatedAddedOK, FileName = string.Empty });
                    break;

                case StatesEnum.ConnectionAddedSuccessfully:
                    MessagesList.Add(new MessageItem { Message = Messages.msgServiceAddedOK, FileName = string.Empty });
                    break;

                default:
                    MessagesList.Add(new MessageItem { Message = Messages.msgServiceAddedOK, FileName = string.Empty });
                    break;

            }
        }

        /// <summary>
        ///  Handle the message display after service Activated
        /// </summary>
        /// <param name="url"></param>
        public void OnServiceActivated(string url)
        {
            MessagesList.Add(new MessageItem { Message = Messages.msgServiceActivated, FileName = string.Empty });             
        }

        /// <summary>
        ///  Handle the message display after service tested (ping)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pingOk"></param>
        public void OnServiceChecked(string url, bool pingOk)
        {
            MessagesList.Add(new MessageItem { Message = (pingOk) ? Messages.msgPingServiceSuccess : Messages.msgPingServiceFailure, FileName = string.Empty });             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void OnServiceRemoved(string url)
        {
            MessagesList.Add(new MessageItem { Message = Messages.msgServiceRemoved, FileName = string.Empty });        
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnSettingsSaved()
        {

            MessagesList.Add(new MessageItem { Message = Messages.msgSettingsSaved, FileName = string.Empty });
        }

        /// <summary>
        ///  Handle the message display when error occurs 
        /// </summary>
        /// <param name="exception"></param>
        public void OnErrorOccured(Exception exception)
        {
            if (exception is WebException)
                MessagesList.Add(new MessageItem { Message = Messages.msgWSError, FileName = string.Empty }); 
            else
                MessagesList.Add(new MessageItem { Message = Messages.msgGenericError, FileName = string.Empty });
            
        }
      
        /// <summary>
        /// 
        /// </summary>
        public void SetBusyMode(bool isBusy)
        {           
            IsBusy = isBusy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage"></param>
        /// <param name="message"></param>
        /// <param name="timeSpan"></param>
        public void OnStepDone(double percentage, string message, TimeSpan timeSpan)
        {
            ProgressPercentage += percentage;
            lock (MessagesList)
            {
                MessagesList.Add(new MessageItem { Message = $"{message}({timeSpan})", FileName = string.Empty }); 
            }
        }
        #endregion
    }
}
