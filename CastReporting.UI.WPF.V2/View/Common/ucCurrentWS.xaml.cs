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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CastReporting.UI.WPF.Common
{
    /// <summary>
    /// Interaction logic for ucHeader.xaml
    /// </summary>
    public partial class UcCurrentWS : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Login
        {
            get { return (string)GetValue(LoginProperty); }
            set { SetValue(LoginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Login.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoginProperty =
            DependencyProperty.Register("Login", typeof(string), typeof(UcCurrentWS), new PropertyMetadata(string.Empty));

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(UcCurrentWS), new PropertyMetadata(string.Empty));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(UcCurrentWS), new PropertyMetadata(string.Empty));

        private bool _isApiKey;
        public bool ApiKey
        {
            get { return _isApiKey; }
            set
            {
                if (_isApiKey == value) return;
                _isApiKey = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("_isApiKey"));
            }
        }
    
        public static readonly DependencyProperty ApiKeyProperty =
            DependencyProperty.Register("ApiKey", typeof(bool), typeof(UcCurrentWS), new PropertyMetadata(false));

        private void CbApiKey_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ApiKey = CbApiKey.IsChecked ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        public UcCurrentWS()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
            CbApiKey.DataContext = this;
        }
    }
}
