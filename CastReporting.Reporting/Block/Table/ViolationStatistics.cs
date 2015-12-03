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
    [Block("VIOLATION_STATISTICS")]
    class ViolationStatistics : TableBlock
    {   
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            #region METHODS

            TableDefinition resultTable = null;
            if (reportData != null && reportData.CurrentSnapshot != null)
            {
                double? criticalViolation = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesNumber);
                double? numCritPerFile = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerFileNumber);
                double? numCritPerKLOC = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);

                double? veryHighCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,Constants.
                                                                                                        QualityDistribution.DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                                                                                                        Constants.ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.ComplexityViolations_VeryHigh.GetHashCode());
                
                double? highCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                                    Constants.QualityDistribution.DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                                                                                                    Constants.ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.ComplexityViolations_HighCost.GetHashCode());

                double? veryHighCostComplexityArtefacts = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                                       Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                                       Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());
                
                double? highCostComplexityArtefacts = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                                   Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                                   Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());

        
                double? nbComplexityArtefacts = MathUtility.GetSum(veryHighCostComplexityArtefacts, highCostComplexityArtefacts);
                double? nbComplexityArtefactsViolation = MathUtility.GetSum(veryHighCostComplexityViolations, highCostComplexityViolations);



                const string metricFormat = "N0";
                const string metricFormatPrecision = "N2";
                var rowData = new List<string>() 
                { 
                    "Name", "Number", "Critical Violations"
                    , (criticalViolation.HasValue) ? criticalViolation.Value.ToString(metricFormat):Constants.No_Value

                    , "  per File", 
                    (numCritPerFile.HasValue)? numCritPerFile.Value.ToString(metricFormatPrecision):Constants.No_Value

                    , "  per kLOC", 
                    (numCritPerKLOC.HasValue)? numCritPerKLOC.Value.ToString(metricFormatPrecision):Constants.No_Value

                    , "Complex Objects",
                    (nbComplexityArtefacts.HasValue)?  nbComplexityArtefacts.Value.ToString(metricFormat):Constants.No_Value
                
                    , "  with violations",
                    (nbComplexityArtefactsViolation.HasValue)? nbComplexityArtefactsViolation.Value.ToString(metricFormat):Constants.No_Value
                };
                
                resultTable = new TableDefinition
                {
                    HasRowHeaders = true,
                    HasColumnHeaders = false,
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
