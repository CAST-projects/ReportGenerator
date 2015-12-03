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
        private bool _HasError;
        public bool HasError
        {
            get
            {
                return _HasError;
            }
            set
            {
                _HasError = value;
                base.OnPropertyChanged("HasError");
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

                base.OnPropertyChanged("TemplatePath");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Int32 ApplicationSizeLimitSupSmall
        {
            get
            {
                return Setting.ReportingParameter.ApplicationSizeLimitSupSmall;
            }
            set
            {
                Setting.ReportingParameter.ApplicationSizeLimitSupSmall = value;

                base.OnPropertyChanged("ApplicationSizeLimitSupSmall");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Int32 ApplicationSizeLimitSupMedium
        {
            get
            {
                return Setting.ReportingParameter.ApplicationSizeLimitSupMedium;
            }
            set
            {
                Setting.ReportingParameter.ApplicationSizeLimitSupMedium = value;

                base.OnPropertyChanged("ApplicationSizeLimitSupMedium");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ApplicationSizeLimitSupLarge
        {
            get
            {
                return Setting.ReportingParameter.ApplicationSizeLimitSupLarge;
            }
            set
            {
                Setting.ReportingParameter.ApplicationSizeLimitSupLarge = value;

                base.OnPropertyChanged("ApplicationSizeLimitSupLarge");
            }
        }


        /// <summary>
        /// Setting.
        /// </summary>
        public Double ApplicationQualityVeryLow
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityVeryLow;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityVeryLow = value;

                base.OnPropertyChanged("ApplicationQualityVeryLow");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double ApplicationQualityLow
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityLow;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityLow = value;

                base.OnPropertyChanged("ApplicationQualityLow");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Double ApplicationQualityMedium
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityMedium;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityMedium = value;

                base.OnPropertyChanged("ApplicationQualityMedium");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Double ApplicationQualityGood
        {
            get
            {
                return Setting.ReportingParameter.ApplicationQualityGood;
            }
            set
            {
                Setting.ReportingParameter.ApplicationQualityGood = value;

                base.OnPropertyChanged("ApplicationQualityGood");
            }
        }

        /// <summary>
        /// 
        /// </summary>      
        public CultureInfo Culture
        {
            get
            {
                return (!string.IsNullOrEmpty(Setting.ReportingParameter.CultureName)) ? new CultureInfo(Setting.ReportingParameter.CultureName) : null;
            }
            set
            {
                Setting.ReportingParameter.CultureName = value.Name;

                base.OnPropertyChanged("Culture");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<CultureInfo> _Cultures;
        public List<CultureInfo> Cultures
        {
            get
            {
                return _Cultures;
            }
            set
            {
                _Cultures = value;

                base.OnPropertyChanged("Cultures");
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

            base.MessageManager.OnSettingsSaved();

            if (!Thread.CurrentThread.CurrentCulture.Name.Equals(Setting.ReportingParameter.CultureName))
            {
                Thread.CurrentThread.CurrentCulture = Culture;
                Thread.CurrentThread.CurrentUICulture = Culture;

                if (LangageChanged != null) LangageChanged(this, null);
            }
        }
    }
}
