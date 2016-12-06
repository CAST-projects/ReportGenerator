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
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CastReporting.UI.WPF.Common
{
    /// <summary>
    /// Interaction logic for ucHeader.xaml
    /// </summary>
    public partial class UcMessage : UserControl
    {

        
        /// <summary>
        /// 
        /// </summary>
        public ICommand ClearCommand { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<MessageItem> Messages
        {
            get
            {
                return (ObservableCollection<MessageItem>)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }


       
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageItem>), typeof(UcMessage), new PropertyMetadata(null));

        /// <summary>
        /// /
        /// </summary>
        public UcMessage()
        {
            InitializeComponent();

            ClearCommand = new CommandHandler(ExecuteClearCommand, null);        
        }


        /// <summary>
        /// 
        /// </summary>
        private void ExecuteClearCommand(object prameter)
        {
            Messages.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenButtonClicked(object sender, RoutedEventArgs e)
        {

            var fileName = ((e.Source as Button)?.DataContext as MessageItem)?.FileName;

            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))  Process.Start(fileName);
        }



        /// <summary>
        ///  Refresh all the bindings on controls
        /// </summary>
        public void Refresh()
        {
            BindingOperations.GetBindingExpression(TxtTitle, TextBlock.TextProperty)?.UpdateTarget();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class MessageItem
    {  
        /// <summary>
        /// 
        /// </summary>
        public string Message{get;set;}


        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MessageItem()
        {
            Date = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Date:HH:mm} - {Message}";
        }
    }
}
