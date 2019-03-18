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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{ 
    [Block("CAST_HIGH_DISTRIBUTION")]
    public class CastHighDistribution : TableBlock
    {
        private const string MetricFormat = "N0";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition back = new TableDefinition();
            int parId;
            Constants.QualityDistribution distributionId;
            List<string> rowData = new List<string>();

            double? previousHttVal = null;

            if (null != options && options.ContainsKey("PAR") && int.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;

            if (null == reportData) return back;

            #region Selected Snapshot

            double? selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"low");
            double? selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"average");
            double? selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"high");
            double? selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"very_high");
            
            double? selectedTotal = (selectedLowVal.HasValue && selectedAveVal.HasValue && selectedHigVal.HasValue && selectedVhiVal.HasValue) ? selectedLowVal.Value + selectedAveVal.Value + selectedHigVal.Value + selectedVhiVal.Value : (double?)null;
            double? selectedHttVal = (selectedHigVal.HasValue && selectedVhiVal.HasValue) ? selectedHigVal.Value + selectedVhiVal.Value : (double?)null;
 
            #endregion Selected Snapshot
            
            #region Previous Snapshot

            if (reportData.PreviousSnapshot != null)
            {

                var previousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                    distributionId.GetHashCode(),"high");
                var previousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                    distributionId.GetHashCode(),"very_high");

                previousHttVal = previousHigVal.HasValue && previousVhiVal.HasValue ? previousHigVal.Value + previousVhiVal.Value : (double?)null;

            }
             
            #endregion Previous Snapshot

            #region Data
            int? variation = (selectedHttVal.HasValue && previousHttVal.HasValue) ? (int)(selectedHttVal - previousHttVal) : (int?)null;
                 
            string distributionName = CastComplexityUtility.GetCostComplexityName(reportData.CurrentSnapshot, distributionId.GetHashCode());

            rowData.AddRange(new[] { distributionName, Labels.Current, Labels.Previous, Labels.Evol, Labels.TotalPercent });
            rowData.AddRange(new[]
            { Labels.ComplexityHighAndVeryHigh
                , selectedHttVal?.ToString(MetricFormat) ?? Constants.No_Value
                , previousHttVal?.ToString(MetricFormat) ?? Constants.No_Value
                , variation.HasValue? FormatEvolution(variation.Value): Constants.No_Value
                , (selectedHttVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)? FormatPercent(selectedHttVal.Value / selectedTotal.Value, false): Constants.No_Value
            });

              
            #endregion Data

            back = new TableDefinition
            {
                Data = rowData,
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbColumns = 5,
                NbRows = 2
            };

            return back;
        }


       
    }
}
