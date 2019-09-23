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
using CastReporting.BLL;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace CastReporting.UI.WPF.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsVM : ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler LangageChanged;

        /// <summary>
        /// 
        /// </summary>
        private bool _hasError;
        public bool HasError
        {
            get
            {
                return _hasError;
            }
            set
            {
                _hasError = value;
                OnPropertyChanged("HasError");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TemplatePath
        {
            get
            {
                return Setting.ReportingParameter.TemplatePath;
            }
            set
            {
                Setting.ReportingParameter.TemplatePath = value;

                OnPropertyChanged("TemplatePath");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int ApplicationSizeLimitSupSmall
        {
            get
            {
                return Setting.ReportingParameter.ApplicationSizeLimitSupSmall;
            }
            set
            {
                Setting.ReportingParameter.ApplicationSizeLimitSupSmall = value;

                OnPropertyChanged("ApplicationSizeLimitSupSmall");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int ApplicationSizeLimitSupMedium
        {
            get
            {
                return Setting.ReportingParameter.ApplicationSizeLimitSupMedium;
            }
            set
            {
                Setting.ReportingParameter.ApplicationSizeLimitSupMedium = value;

                OnPropertyChanged("ApplicationSizeLimitSupMedium");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ApplicationSizeLimitSupLarge
        {
            get
            {
                return Setting.ReportingParameter.ApplicationSizeLimitSupLarge;
            }
            set
            {
                Setting.ReportingParameter.ApplicationSizeLimitSupLarge = value;

                OnPropertyChanged("ApplicationSizeLimitSupLarge");
            }
        }


        /// <summary>
        /// Setting.
        /// </summary>
        public double ApplicationQualityVeryLow
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityVeryLow;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityVeryLow = value;

                OnPropertyChanged("ApplicationQualityVeryLow");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityLow
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityLow;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityLow = value;

                OnPropertyChanged("ApplicationQualityLow");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityMedium
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityMedium;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityMedium = value;

                OnPropertyChanged("ApplicationQualityMedium");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double ApplicationQualityGood
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityGood;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityGood = value;

                OnPropertyChanged("ApplicationQualityGood");
            }
        }

        /// <summary>
        /// 
        /// </summary>      
        public CultureInfo Culture
        {
            get
            {
                return !string.IsNullOrEmpty(Setting.ReportingParameter.CultureName) ? new CultureInfo(Setting.ReportingParameter.CultureName) : null;
            }
            set
            {
                //string previousCulture = Setting.ReportingParameter.CultureName;
                Setting.ReportingParameter.CultureName = value.Name;
                /*
                if (value.Name.Equals("zh-CN"))
                {
                    string previousTemplatePath = Setting.ReportingParameter.TemplatePath;
                    TemplatePath = previousTemplatePath + "\\zh-CN";
                }
                
                if (previousCulture.Equals("zh-CN"))
                {
                    string previousTemplatePath = Setting.ReportingParameter.TemplatePath;
                    int idxToKeep = previousTemplatePath.Length - "\\zh-CN".Length;
                    TemplatePath = previousTemplatePath.Substring(0, idxToKeep);
                }
                */
                OnPropertyChanged("Culture");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<CultureInfo> _cultures;
        public List<CultureInfo> Cultures
        {
            get
            {
                return _cultures;
            }
            set
            {
                _cultures = value;

                OnPropertyChanged("Cultures");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public SettingsVM()
        {
            Cultures = Properties.Settings.Default.Cultures.Split(';').Select(_ => new CultureInfo(_)).ToList();
        }

        /// <summary>
        /// Implement Add service Command
        /// </summary>
        public void SaveSettings()
        {
            SettingsBLL.SaveSetting(Setting);

            MessageManager.OnSettingsSaved();

            if (Thread.CurrentThread.CurrentCulture.Name.Equals(Setting.ReportingParameter.CultureName)) return;
            Thread.CurrentThread.CurrentCulture = Culture;
            Thread.CurrentThread.CurrentUICulture = Culture;

            LangageChanged?.Invoke(this, null);
        }
    }
}
