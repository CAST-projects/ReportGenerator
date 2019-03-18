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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Core.Languages;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CAST_DISTRIBUTION")]
    public class CastDistribution : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition result = new TableDefinition();
            List<string> rowData = new List<string>();
            int parId;
            Constants.QualityDistribution distributionId;
            double? selectedTotal = null;

            if (null != options && options.ContainsKey("PAR") && int.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;


            if (reportData?.CurrentSnapshot == null) return result;

            #region Selected Snapshot

            var selectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"low");
            var selectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"average");
            var selectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(), "high");
            var selectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                distributionId.GetHashCode(),"very_high");
            
            if (selectedLowVal.HasValue && selectedAveVal.HasValue && selectedHigVal.HasValue && selectedVhiVal.HasValue)
                selectedTotal = selectedLowVal + selectedAveVal + selectedHigVal + selectedVhiVal;

            #endregion Selected Snapshot

            #region Previous Snapshot

            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;
            
            if (reportData.PreviousSnapshot != null)
            {                    
                previousLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                    distributionId.GetHashCode(), "low");
                previousAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                    distributionId.GetHashCode(), "average");
                previousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                    distributionId.GetHashCode(),"high");
                previousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                    distributionId.GetHashCode(),"very_high");

            }

            #endregion Previous Snapshot

            #region Data
            var distributionName = CastComplexityUtility.GetCostComplexityName(reportData.CurrentSnapshot, distributionId.GetHashCode());

            rowData.AddRange(new[] { distributionName, Labels.Current, Labels.Previous, Labels.Evol, Labels.EvolPercent, Labels.TotalPercent });
                
            rowData.AddRange(new[]
            { Labels.ComplexityLow
                , selectedLowVal?.ToString("N0") ?? Constants.No_Value
                , previousLowVal?.ToString("N0") ?? Constants.No_Value
                , (selectedLowVal.HasValue && previousLowVal.HasValue) ? FormatEvolution((int)(selectedLowVal.Value - previousLowVal.Value)): Constants.No_Value
                , (selectedLowVal.HasValue && previousLowVal.HasValue && Math.Abs(previousLowVal.Value) > 0)? FormatPercent((selectedLowVal - previousLowVal) / previousLowVal): Constants.No_Value
                , (selectedLowVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?FormatPercent(selectedLowVal / selectedTotal, false): Constants.No_Value
            });

            rowData.AddRange(new[]
            { Labels.ComplexityAverage
                , selectedAveVal?.ToString("N0") ?? Constants.No_Value
                , previousAveVal?.ToString("N0") ?? Constants.No_Value
                , (selectedAveVal.HasValue && previousAveVal.HasValue) ? FormatEvolution((int)(selectedAveVal.Value - previousAveVal.Value)) : Constants.No_Value
                , (selectedAveVal.HasValue && previousAveVal.HasValue && Math.Abs(previousAveVal.Value) > 0)? FormatPercent((selectedAveVal - previousAveVal) / previousAveVal): Constants.No_Value
                , (selectedAveVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?FormatPercent(selectedAveVal / selectedTotal, false): Constants.No_Value
            });

            rowData.AddRange(new[]
            { Labels.ComplexityHigh
                , selectedHigVal?.ToString("N0") ?? Constants.No_Value
                , previousHigVal?.ToString("N0") ?? Constants.No_Value
                , previousHigVal.HasValue ? FormatEvolution((int)(selectedHigVal.Value - previousHigVal.Value)): Constants.No_Value
                , (selectedHigVal.HasValue && previousHigVal.HasValue && Math.Abs(previousHigVal.Value) > 0)? FormatPercent((selectedHigVal - previousHigVal) / previousHigVal): Constants.No_Value
                , (selectedHigVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?FormatPercent(selectedHigVal / selectedTotal, false): Constants.No_Value
            });

            rowData.AddRange(new[]
            { Labels.ComplexityExtreme
                , selectedVhiVal?.ToString("N0") ?? Constants.No_Value
                , previousVhiVal?.ToString("N0") ?? Constants.No_Value
                , (selectedVhiVal.HasValue && previousVhiVal.HasValue) ? FormatEvolution((int)(selectedVhiVal.Value - previousVhiVal.Value)): Constants.No_Value
                , (selectedVhiVal.HasValue && previousVhiVal.HasValue && Math.Abs(previousVhiVal.Value) > 0)? FormatPercent((selectedVhiVal - previousVhiVal) / previousVhiVal): Constants.No_Value
                , (selectedVhiVal.HasValue && selectedTotal.HasValue && selectedTotal.Value>0)?FormatPercent(selectedVhiVal / selectedTotal, false): Constants.No_Value
            });
                
            #endregion Data

            result = new TableDefinition
            {
                Data = rowData,
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbColumns = 6,
                NbRows = 5
            };
            return result;
        }
    }
}
