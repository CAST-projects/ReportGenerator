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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CastReporting.Domain;
using CastReporting.Repositories.Interfaces;
using CastReporting.Repositories.Properties;
using CastReporting.Repositories.Util;
using Microsoft.Win32;

namespace CastReporting.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsRepository : ISettingRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void SaveSetting(Setting setting)
        {
            string settingFilePath = Path.Combine(this.GetApplicationPath(), Settings.Default.SettingFileName);
                       
            SerializerHelper.SerializeToFile<Setting>(setting,settingFilePath);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Setting GetSeting()
        {
            Setting setting;

            string settingFilePath = Path.Combine(this.GetApplicationPath(), Settings.Default.SettingFileName);

            if (File.Exists(settingFilePath))
            {
                setting = SerializerHelper.DeserializeFromFile<Setting>(settingFilePath);
            }
            else
            {
                setting = new Setting();
            }
            
            if(string.IsNullOrEmpty(setting.ReportingParameter.TemplatePath))
                setting.ReportingParameter.TemplatePath = Path.Combine(this.GetApplicationPath(), Settings.Default.TemplateDirectory);
            return setting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        List<FileInfo> ISettingRepository.GetTemplateFileList(string tempaltePath)
        {
            List<FileInfo> result = new List<FileInfo>();

            if (string.IsNullOrEmpty(tempaltePath)) return result;

            DirectoryInfo di = new DirectoryInfo(tempaltePath);
            if (di.Exists)
            {
                var extensions = Settings.Default.TemplateExtensions.Split(',');
                result = di.GetFiles().Where(f => extensions.Contains(Path.GetExtension(f.FullName).ToLower())).ToList();
            }

            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetApplicationPath()
        {
            Version vers = Assembly.GetExecutingAssembly().GetName().Version;
            String version = vers.Major.ToString() + '.' + vers.Minor.ToString() + '.' + vers.Revision.ToString();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Settings.Default.CompanyName, Settings.Default.ProductName, version);

            // Create Folder if not exists
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }


        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {

        }              
     

      
    }
}
