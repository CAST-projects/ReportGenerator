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
using System.Windows;
using System.Windows.Controls;
using CastReporting.UI.WPF.View;
using CastReporting.UI.WPF.ViewModel;
using System.Windows.Input;
using CastReporting.UI.WPF.Resources.Languages;
using System.Reflection;

namespace CastReporting.UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public string Messges { get; set; }


        /// <summary>
        /// 
        /// </summary>
        private const string NavigationPath = "View/Pages/{0}";

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowVM();
            MainFrame.Navigate(new Uri(string.Format(NavigationPath, "reporting.xaml"), UriKind.RelativeOrAbsolute));
            MainFrame.Navigated += OnFrameNavigated;

            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            Title = ver.Build == 0 ? $"{Messages.lblTitleMain}-{ver.Major}.{ver.Minor}" : $"{Messages.lblTitleMain}-{ver.Major}.{ver.Minor}.{ver.Build}";

            EventManager.RegisterClassHandler(typeof(TextBox), GotFocusEvent, new RoutedEventHandler(TextBox_SelectAllText));
            EventManager.RegisterClassHandler(typeof(TextBox), PreviewMouseDownEvent, new MouseButtonEventHandler(TextBox_SelectivelyIgnoreMouseButton));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TextBox_SelectAllText(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TextBox_SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // If its a triple click, select all text for the user.
            if (e.ClickCount == 3)
            {
                TextBox_SelectAllText(sender, new RoutedEventArgs());
                return;
            }

            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }

            // ReSharper disable once IsExpressionAlwaysTrue
            if (!(parent is TextBox)) return;
            var textBox = (TextBox) parent;
            if (textBox.IsKeyboardFocusWithin) return;
            // If the text box is not yet focussed, give it the focus and
            // stop further processing of this click event.
            textBox.Focus();
            e.Handled = true;
        }

        /// <summary>
        /// Uptade the acitvated WS in the header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFrameNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var selectPage = e.Content as Page;

            if (selectPage == null) return;
            var dataContext = selectPage.DataContext as ViewModelBase;

            if (dataContext != null)
                dataContext.MessageManager = DataContext as IMessageManager;

            if (selectPage is Settings) (selectPage as Settings).LangageChanged += OnLanguageChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLanguageChanged(object sender, RoutedEventArgs e)
        {
            UcMessages.Refresh();
            UcHeader.Refresh();
            MainFrame.NavigationService.Refresh();
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnCommandMenuExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var page = e.Parameter as string;
            if (!string.IsNullOrWhiteSpace(page))
            {
                MainFrame.Navigate(new Uri(string.Format(NavigationPath, page), UriKind.RelativeOrAbsolute));
            }

            e.Handled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnQuitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
       
    }
}
