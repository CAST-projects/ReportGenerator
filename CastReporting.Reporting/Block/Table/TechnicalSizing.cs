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
using System.Linq;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNICAL_SIZING")]
    class TechnicalSizing : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
              #region METHODS
           TableDefinition resultTable = null;
            if (null != reportData &&
                null != reportData.CurrentSnapshot) {
                double? codeLineNumber = MeasureUtility.GetCodeLineNumber(reportData.CurrentSnapshot);
                double? fileNumber = MeasureUtility.GetFileNumber(reportData.CurrentSnapshot);
                double? classNumber = MeasureUtility.GetClassNumber(reportData.CurrentSnapshot);
                double? sqlArtifactNumber = MeasureUtility.GetSqlArtifactNumber(reportData.CurrentSnapshot);
                double? tableNumber = MeasureUtility.GetTableNumber(reportData.CurrentSnapshot);

				const string noData = Constants.No_Data;
                const string metricFormat = "N0";
                var rowData = new List<string>() { Labels.Name, Labels.Value
                    , Labels.kLoC, (codeLineNumber.HasValue ? (codeLineNumber.Value / 1000).ToString(metricFormat) : noData)
                    , "  " + Labels.Files, (fileNumber.HasValue ? (fileNumber.Value).ToString(metricFormat) : noData)
                    , "  " + Labels.Classes, (classNumber.HasValue ? (classNumber.Value).ToString(metricFormat) : noData)
                    , Labels.ArtifactsSQL, (sqlArtifactNumber.HasValue ? (sqlArtifactNumber.Value).ToString(metricFormat) : noData)
                    , "  " + Labels.Tables, (tableNumber.HasValue ? (tableNumber.Value).ToString(metricFormat) : noData)
                    };
                resultTable = new TableDefinition {
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
