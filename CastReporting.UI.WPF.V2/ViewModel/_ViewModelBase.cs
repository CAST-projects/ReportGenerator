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
using CastReporting.BLL;
using CastReporting.Domain;
using System;
using Cast.Util;

namespace CastReporting.UI.WPF.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ViewModelBase : NotifyPropertyChangedHelper
    {


        /// <summary>
        /// 
        /// </summary>
        public static Setting Setting {get; protected set;}

        /// <summary>
        /// 
        /// </summary>
        public IMessageManager MessageManager { set; get; }


        /// <summary>
        /// 
        /// </summary>
        static  ViewModelBase()
        {
            Setting = SettingsBLL.GetSetting();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        protected static void WorkerThreadException(CastReportingException ex)
        {
            throw ex;
        }
    }
}
 