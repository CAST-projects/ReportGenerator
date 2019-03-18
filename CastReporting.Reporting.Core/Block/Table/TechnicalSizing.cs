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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNICAL_SIZING")]
    public class TechnicalSizing : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
              #region METHODS

            if (reportData?.CurrentSnapshot == null) return null;
            double? codeLineNumber = MeasureUtility.GetCodeLineNumber(reportData.CurrentSnapshot);
            double? fileNumber = MeasureUtility.GetFileNumber(reportData.CurrentSnapshot);
            double? classNumber = MeasureUtility.GetClassNumber(reportData.CurrentSnapshot);
            double? sqlArtifactNumber = MeasureUtility.GetSqlArtifactNumber(reportData.CurrentSnapshot);
            double? tableNumber = MeasureUtility.GetTableNumber(reportData.CurrentSnapshot);

            const string noData = Constants.No_Value;
            const string metricFormat = "N0";
            var rowData = new List<string>{ Labels.Name, Labels.Value
                , Labels.kLoC, (codeLineNumber / 1000)?.ToString(metricFormat) ?? noData
                , "  " + Labels.Files, fileNumber?.ToString(metricFormat) ?? noData
                , "  " + Labels.Classes, classNumber?.ToString(metricFormat) ?? noData
                , Labels.ArtifactsSQL, sqlArtifactNumber?.ToString(metricFormat) ?? noData
                , "  " + Labels.Tables, tableNumber?.ToString(metricFormat) ?? noData
            };
            var resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 6,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
