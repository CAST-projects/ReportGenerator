using System;
using System.Collections.Generic;
using System.IO;
using CastReporting.Domain;


namespace CastReporting.Repositories.Interfaces
{
    /// <summary>
    /// Defines the minimal data access layer methods that must be
    /// enabled in class that inherit of this class.
    /// </summary>
    public interface ISettingRepository : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void SaveSetting(Setting setting);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Setting GetSeting();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<FileSystemInfo> GetTemplateFileList(string templateFilePath);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetApplicationPath();
    }
}
