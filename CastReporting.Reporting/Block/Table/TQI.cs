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
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TQI")]
    class TQI : TableBlock
    {
         /// <summary>
        /// 
        /// </summary>
        private const string _MetricFormat = "N2";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Statistics, Labels.Current, Labels.Previous });


            Double? currentTqi= BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(reportData.CurrentSnapshot, Domain.Constants.BusinessCriteria.TechnicalQualityIndex, true);
            Double? previousTqi = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(reportData.PreviousSnapshot, Domain.Constants.BusinessCriteria.TechnicalQualityIndex, true);

            rowData.AddRange(new[] { Labels.TQI, 
                                       currentTqi.HasValue ? currentTqi.Value.ToString(_MetricFormat): String.Empty, 
                                       previousTqi.HasValue ? previousTqi.Value.ToString(_MetricFormat):CastReporting.Domain.Constants.No_Value});

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 2,
                NbColumns = 3,
                Data = rowData
            };

            return resultTable;
        }
    }
}
