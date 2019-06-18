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
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNO_LOC_BY_MODULE")]
    public class TechnoLoCModule : TableBlock
    {
        /// <summary>
        /// 
        /// </summary>
        private const string MetricFormat = "N0";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string> {""};

            //Set the table header
            rowData.AddRange(reportData.CurrentSnapshot.Technologies);

            //Set the result by module
            foreach (var mod in reportData.CurrentSnapshot.Modules)
            {
                var technologyLoc = MeasureUtility.GetSizingMeasureTechnologies(reportData.CurrentSnapshot, mod.Href, Constants.SizingInformations.CodeLineNumber.GetHashCode());

                rowData.Add(mod.Name);

                rowData.AddRange(reportData.CurrentSnapshot.Technologies.Select(techName => technologyLoc.FirstOrDefault(_ => _.Name == techName)).Select(result => result != null ? result.Value?.ToString(MetricFormat) : Constants.No_Value));
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = reportData.CurrentSnapshot.Modules.Count() + 1,
                NbColumns = reportData.CurrentSnapshot.Technologies.Length  + 1,
                Data = rowData
            };

            return resultTable;
        }



       
    }
}
