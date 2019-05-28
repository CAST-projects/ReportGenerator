
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
using System.IO;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Repositories;
using CastReporting.Repositories.Interfaces;

namespace CastReporting.BLL
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsBLL
    {
        private SettingsBLL()
        {
            // Avoid instanciation of the class
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<FileInfo> GetTemplateFileList()
        {
            using (ISettingRepository setttingRepository = new SettingsRepository())
            {
                string templateFilePath = setttingRepository.GetSeting().ReportingParameter.TemplatePath;

                return setttingRepository.GetTemplateFileList(templateFilePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<FileInfo> GetTemplateFileListPortfolio()
        {
            using (ISettingRepository setttingRepository = new SettingsRepository())
            {
                ReportingParameter rp = new ReportingParameter();
                string templateFilePath = setttingRepository.GetSeting().ReportingParameter.TemplatePath;
                return setttingRepository.GetTemplateFileList(templateFilePath + rp.PortfolioFolderNamePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationPath()
        {
            using (ISettingRepository setttingRepository = new SettingsRepository())
            {
                return setttingRepository.GetApplicationPath();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Setting GetSetting()
        {
            using (ISettingRepository setttingRepository = new SettingsRepository())
            {
                return setttingRepository.GetSeting();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static void SaveSetting(Setting setting)
        {
            using (ISettingRepository setttingRepository = new SettingsRepository())
            {
                setttingRepository.SaveSetting(setting);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Setting AddConnection(WSConnection connection, bool isActive, out StatesEnum state)
        {
            using (ISettingRepository settingRepository = new SettingsRepository())
            {
                var setting = settingRepository.GetSeting();

                if (!CommonBLL.CheckService(connection))
                {
                    state = StatesEnum.ServiceInvalid;
                    return setting;
                }

                if (!setting.WSConnections.Any(x => x.Equals(connection)))
                {
                    setting.WSConnections.Add(connection);
                }
                else
                {
                    WSConnection existing = setting.WSConnections?.Where(x => x.Equals(connection)).FirstOrDefault();
                    if (existing?.Login == connection.Login && existing?.Password == connection.Password && existing?.ApiKey == connection.ApiKey)
                    {
                        // ReSharper disable once RedundantAssignment
                        state = StatesEnum.ConnectionAlreadyExist;
                    }
                    else
                    {
                        setting.WSConnections.Remove(existing);
                        setting.WSConnections.Add(connection);
                    }

                }


                if (isActive)
                {
                    setting.ChangeActiveConnection(connection.Uri.ToString());
                    state = StatesEnum.ConnectionAddedAndActivated;
                }
                else
                {
                    state = StatesEnum.ConnectionAddedSuccessfully;
                }

                settingRepository.SaveSetting(setting);

                return setting;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Setting RemoveConnection(WSConnection connection)
        {
            using (ISettingRepository setttingRepository = new SettingsRepository())
            {
                var setting = setttingRepository.GetSeting();
                setting.WSConnections.Remove(connection);
                setttingRepository.SaveSetting(setting);

                return setting;
            }
        }
    }
}
