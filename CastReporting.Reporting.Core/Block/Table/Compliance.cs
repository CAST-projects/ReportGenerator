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
    [Block("COMPLIANCE")]
    public class Compliance : TableBlock
    {   

         #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {

          
            const string metricFormat = "N2";
            TableDefinition resultTable = null;
            bool displayShortHeader = (options != null && options.ContainsKey("HEADER") && "SHORT" == options["HEADER"]);

            if (reportData?.CurrentSnapshot?.BusinessCriteriaResults == null) return resultTable;

            bool hasPreviousSnapshot = reportData.PreviousSnapshot?.BusinessCriteriaResults != null;
            string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
            BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.CurrentSnapshot, false);


            string prevSnapshotLabel = hasPreviousSnapshot ? SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot) : Domain.Constants.No_Value;
            BusinessCriteriaDTO prevSnapshotBisCriDTO = hasPreviousSnapshot ? BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(reportData.PreviousSnapshot, false) : null;


            double? currProgrammingPracticesValue = currSnapshotBisCriDTO.ProgrammingPractices ?? 1;
            double? currArchitecturalDesignValue = currSnapshotBisCriDTO.ArchitecturalDesign ?? 1;
            double? currDocumentationValue = currSnapshotBisCriDTO.Documentation ?? 1;


            double? prevProgrammingPracticesValue = hasPreviousSnapshot?prevSnapshotBisCriDTO.ProgrammingPractices : 0;
            double? prevArchitecturalDesignValue = hasPreviousSnapshot ?prevSnapshotBisCriDTO.ArchitecturalDesign : 0;
            double? prevDocumentationValue = hasPreviousSnapshot?prevSnapshotBisCriDTO.Documentation : 0;

            double? varProgrammingPractices = MathUtility.GetPercent(MathUtility.GetEvolution(currProgrammingPracticesValue.Value, prevProgrammingPracticesValue.Value),
                prevProgrammingPracticesValue.Value);

            double? varArchitecturalDesign = MathUtility.GetPercent(MathUtility.GetEvolution(currArchitecturalDesignValue.Value, prevArchitecturalDesignValue.Value),
                prevArchitecturalDesignValue.Value);

            double? varDocumentation = MathUtility.GetPercent(MathUtility.GetEvolution(currDocumentationValue.Value, prevDocumentationValue.Value), 
                prevDocumentationValue.Value);



            List<string> rowData = new List<string>();
            rowData.AddRange(displayShortHeader ? new[] {"", Labels.Prog, Labels.Arch, Labels.Doc} : new[] {"", Labels.ProgrammingPractices, Labels.ArchitecturalDesign, Labels.Documentation});

            rowData.AddRange(
                new[]
                {
                    currSnapshotLabel,
                    currProgrammingPracticesValue?.ToString(metricFormat) ?? Domain.Constants.No_Value,
                    currArchitecturalDesignValue?.ToString(metricFormat) ?? Domain.Constants.No_Value,
                    currDocumentationValue?.ToString(metricFormat) ?? Domain.Constants.No_Value
                });
            if (hasPreviousSnapshot)
            {
                rowData.AddRange(
                    new[]
                    {
                        prevSnapshotLabel,
                        prevProgrammingPracticesValue?.ToString(metricFormat) ?? Domain.Constants.No_Value,
                        prevArchitecturalDesignValue?.ToString(metricFormat) ?? Domain.Constants.No_Value,
                        prevDocumentationValue?.ToString(metricFormat) ?? Domain.Constants.No_Value,
                        Labels.Variation,
                        varProgrammingPractices.HasValue ? FormatPercent(varProgrammingPractices.Value): Domain.Constants.No_Value,
                        varArchitecturalDesign.HasValue ? FormatPercent(varArchitecturalDesign.Value): Domain.Constants.No_Value,
                        varDocumentation.HasValue ? FormatPercent(varDocumentation.Value): Domain.Constants.No_Value
                    });
            }
            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = hasPreviousSnapshot ? 4 : 2,
                NbColumns = 4,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
