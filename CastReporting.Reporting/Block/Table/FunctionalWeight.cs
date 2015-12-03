/*
 *   Copyright (c) 2015 CAST
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
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("FUNCTIONAL_WEIGHT")]
    class FunctionalWeight : TableBlock
    {
        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            if (null != reportData &&
                    null != reportData.CurrentSnapshot)
            {
                double? automatedFPoints = MeasureUtility.GetAutomatedIFPUGFunction(reportData.CurrentSnapshot);
                double? decisionPoints = MeasureUtility.GetDecisionPointsNumber(reportData.CurrentSnapshot);
                double? backFiredFPoints = MeasureUtility.GetBackfiredIFPUGFunction(reportData.CurrentSnapshot);

                const string metricFormat = "N0";
                var rowData = new List<string>() 
                    { "Name", "Number"
                    , "Automated function Points", (automatedFPoints.HasValue ?  automatedFPoints.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value)
                    , "Decision Points (Total CC)",( decisionPoints.HasValue ?  (decisionPoints.Value).ToString(metricFormat) : CastReporting.Domain.Constants.No_Value)
                    , "Backfired Function Points", (backFiredFPoints.HasValue ?  (backFiredFPoints.Value).ToString(metricFormat) :CastReporting.Domain.Constants.No_Value)
                    };
                resultTable = new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 6,
                    NbColumns = 2,
                    Data = rowData
                };
            }
            return resultTable;
        }
        #endregion METHODS
    }
}
