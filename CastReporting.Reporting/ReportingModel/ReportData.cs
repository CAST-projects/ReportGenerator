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
using System.IO;
using CastReporting.Domain;
using CastReporting.Domain.Interfaces;

namespace CastReporting.Reporting.ReportingModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportData : IDisposable
    {

       
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public FormatType ReportType 
        {
            get
            {
                switch(Path.GetExtension(FileName))
                {
                    case ".docx":                    
                        return FormatType.Word; 
                    
                    case ".xlsx":                    
                        return FormatType.Excel; 
                    
                    case ".pptx":
                        return FormatType.PowerPoint; 
                    
                    default:
                        return 0;
                }
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        public Application Application
          { 
            get; 
            set; 
        }

        public string Category
        {
            get;
            set;
        }

        public string Tag
        {
            get;
            set;
        }



         /// <summary>
        /// 
        /// </summary>
        public Snapshot CurrentSnapshot
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Snapshot PreviousSnapshot
        { 
            get; 
            set; 
        }


        /// <summary>
        /// 
        /// </summary>
        public ReportingParameter Parameter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IRuleExplorer RuleExplorer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ISnapshotExplorer SnapshotExplorer { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string CurrencySymbol { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public Application[] Applications
        {
            get;
            set;
        }

        // ReSharper disable once InconsistentNaming
        public Snapshot[] Snapshots
        {
            get;
            set;
        }

        public string[] IgnoresApplications
        {
            get;
            set;
        }

        public string[] IgnoresSnapshots
        {
            get;
            set;
        }

        public void Dispose()
        {
            RuleExplorer?.Dispose();
            SnapshotExplorer?.Dispose();

            GC.SuppressFinalize(this);
        }

        public string ServerVersion { get; set; }
    }
}
