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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CastReporting.Domain;
using CastReporting.Repositories.Core;
using CastReporting.Repositories.Interfaces;
using CastReporting.Repositories.Util;

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
            string settingFilePath = Path.Combine(GetApplicationPath(), Settings.Default.SettingFileName);
                       
            SerializerHelper.SerializeToFile(setting,settingFilePath);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Setting GetSeting()
        {
            string settingFilePath = Path.Combine(GetApplicationPath(), Settings.Default.SettingFileName);

            var setting = File.Exists(settingFilePath) ? SerializerHelper.DeserializeFromFile<Setting>(settingFilePath) : new Setting();

            if (string.IsNullOrEmpty(setting.ReportingParameter.TemplatePath))
            {
                //setting.ReportingParameter.TemplatePath = Path.Combine(GetApplicationPath(), Settings.Default.TemplateDirectory);
                setting.ReportingParameter.TemplatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Settings.Default.TemplateDirectory);
            }
            return setting;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        List<FileInfo> ISettingRepository.GetTemplateFileList(string templatePath)
        {
            List<FileInfo> result = new List<FileInfo>();

            if (string.IsNullOrEmpty(templatePath)) return result;

            DirectoryInfo di = new DirectoryInfo(templatePath);
            if (!di.Exists) return result;
            var extensions = Settings.Default.TemplateExtensions.Split(',');
            result = di.GetFiles().Where(f => extensions.Contains(Path.GetExtension(f.FullName).ToLower())).ToList();

            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetApplicationPath()
        {
            /*
            Version vers = Assembly.GetExecutingAssembly().GetName().Version;
            string version = vers.Major.ToString() + '.' + vers.Minor.ToString() + '.' + vers.Build.ToString();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Settings.Default.CompanyName, Settings.Default.ProductName, version);
            // Create Folder if not exists
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
            */
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }


        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
{

}              



}
}
