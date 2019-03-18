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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Table
{
    [Block("FUNCTIONAL_WEIGHT")]
    public class FunctionalWeight : TableBlock
    {
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (reportData?.CurrentSnapshot == null) return null;
            double? automatedFPoints = MeasureUtility.GetAutomatedIFPUGFunction(reportData.CurrentSnapshot);
            double? decisionPoints = MeasureUtility.GetDecisionPointsNumber(reportData.CurrentSnapshot);
            double? backFiredFPoints = MeasureUtility.GetBackfiredIFPUGFunction(reportData.CurrentSnapshot);

            const string metricFormat = "N0";
            var rowData = new List<string>() 
            {  Labels.Name, Labels.Total
                , Labels.AutomatedFP, automatedFPoints?.ToString(metricFormat) ?? Domain.Constants.No_Value
                , Labels.DecisionP,decisionPoints?.ToString(metricFormat) ?? Domain.Constants.No_Value
                , Labels.BackfiredFP, backFiredFPoints?.ToString(metricFormat) ?? Domain.Constants.No_Value
            };
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 4,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
