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
    [Block("CAST_COMPLEXITY_WITH_VIOL")]
    public class CastComplexityWithViolation : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition back = new TableDefinition();
            List<string> rowData = new List<string>();
            string numberFormat = "N0";

            if (null != reportData && null != reportData.CurrentSnapshot && reportData.CurrentSnapshot.CostComplexityResults != null)
            {
           

                #region Selected Snapshot

                            
                double? nbArtifactLow = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                    Constants.CostComplexity.CostComplexityArtifacts_Low.GetHashCode());
                double? nbArtifactAve = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                    Constants.CostComplexity.CostComplexityArtifacts_Average.GetHashCode());
                double? nbArtifactHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                    Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());
                double? nbArtifactVeryHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                    Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());

                double? nbViolationLow = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                    Constants.DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_Low.GetHashCode());
                double? nbViolationAve = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                    Constants.DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_Average.GetHashCode());
                double? nbViolationHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                    Constants.DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_High.GetHashCode());
                double? nbViolationVeryHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                    Constants.QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                    Constants.DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_VeryHigh.GetHashCode());

                #endregion Selected Snapshot

                #region Data

              
                rowData.AddRange(new string[] { "Cast Complexity", "Artifacts", "w/ violations" });
                rowData.AddRange(new string[] { "Extreme", nbArtifactVeryHigh.Value.ToString(numberFormat), nbViolationVeryHigh.Value.ToString(numberFormat) });
                rowData.AddRange(new string[] { "High", nbArtifactHigh.Value.ToString(numberFormat), nbViolationHigh.Value.ToString(numberFormat) });
                rowData.AddRange(new string[] { "Average", nbArtifactAve.Value.ToString(numberFormat), nbViolationAve.Value.ToString(numberFormat) });
                rowData.AddRange(new string[] { "Low", nbArtifactLow.Value.ToString(numberFormat), nbViolationLow.Value.ToString(numberFormat) });

                #endregion Data

                back = new TableDefinition
                {
                    Data = rowData,
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbColumns = 3,
                    NbRows = 4
                };
            }
            return back;
        }
    }
}
