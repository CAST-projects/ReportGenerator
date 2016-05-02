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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("VIOLATION_STATISTICS_EVOLUTION")]
    class ViolationStatisticsEvolution : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            const string metricFormat = "N0";
            if (reportData != null && reportData.CurrentSnapshot != null)
            {

                #region currentSnapshot

                double? criticalViolation = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesNumber);
                double? numCritPerFile = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerFileNumber);
                string numCritPerFileIfNegative = string.Empty;
                if (numCritPerFile == -1)
                    numCritPerFileIfNegative = "N/A";
                else
                    numCritPerFileIfNegative = (numCritPerFile.HasValue) ? numCritPerFile.Value.ToString("N2") : CastReporting.Domain.Constants.No_Value;
                double? numCritPerKLOC = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);

                double? veryHighCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot, Constants.
                                                                                                         QualityDistribution.DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                                                                                                         Constants.ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.
                                                                                                         ComplexityViolations_VeryHigh.GetHashCode());
                double? highCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                                    Constants.QualityDistribution.DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                                                                                                    Constants.ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.ComplexityViolations_HighCost.GetHashCode());

                double? veryHighCostComplexityArtefacts = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                                        Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                                        Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());
                double? highCostComplexityArtefacts = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                                                                                                    Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                                    Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());

                #endregion currentSnapshot


                #region PreviousSnapshot

                double? criticalViolationPrev = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, 
                                                                                Constants.SizingInformations.ViolationsToCriticalQualityRulesNumber);

                double? numCritPerFilePrev = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, 
                                                                                    Constants.SizingInformations.ViolationsToCriticalQualityRulesPerFileNumber);
                string numCritPerFilePrevIfNegative = string.Empty;
                if (numCritPerFilePrev == -1)
                    numCritPerFilePrevIfNegative = "N/A";
                else
                    numCritPerFilePrevIfNegative = (numCritPerFilePrev.HasValue) ? numCritPerFilePrev.Value.ToString("N2") : CastReporting.Domain.Constants.No_Value;

                double? numCritPerKLOCPrev = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, 
                                                                             Constants.SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);

                double? veryHighCostComplexityViolationsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot, 
                                                                                                            Constants.QualityDistribution.DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                                                                                                            Constants.ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.ComplexityViolations_VeryHigh.GetHashCode());
                
                double? highCostComplexityViolationsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                                        Constants.QualityDistribution.DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                                                                                                        Constants.ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity.ComplexityViolations_HighCost.GetHashCode());

                double? veryHighCostComplexityArtefactsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                                           Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                                           Constants.CostComplexity.CostComplexityArtifacts_VeryHigh.GetHashCode());

                double? highCostComplexityArtefactsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                                                                                                         Constants.QualityDistribution.CostComplexityDistribution.GetHashCode(),
                                                                                                         Constants.CostComplexity.CostComplexityArtifacts_High.GetHashCode());

                #endregion PreviousSnapshot
                #region SumMetric

                double? HighveryHighCostComplexityArtefacts = MathUtility.GetSum(veryHighCostComplexityArtefacts, highCostComplexityArtefacts);
                double? HighveryHighCostComplexityViolations = MathUtility.GetSum(veryHighCostComplexityViolations, highCostComplexityViolations);
                double? HighveryHighCostComplexityArtefactsPrev = MathUtility.GetSum(veryHighCostComplexityArtefactsPrev, highCostComplexityArtefactsPrev);
                double? HighveryHighCostComplexityViolationsPrev = MathUtility.GetSum(veryHighCostComplexityViolationsPrev, highCostComplexityViolationsPrev);

                #endregion SumMetric
              
                #region evolutionPercMetric

                double? criticalViolationEvolPerc = MathUtility.GetVariationPercent(criticalViolation, criticalViolationPrev);
                double? numCritPerFileEvolPerc = MathUtility.GetVariationPercent(numCritPerFile, numCritPerFilePrev);
                double? numCritPerKLOCEvolPerc = MathUtility.GetVariationPercent(numCritPerKLOC, numCritPerKLOCPrev);
                double? HighveryHighCostComplexityViolationsEvolPerc = MathUtility.GetVariationPercent(HighveryHighCostComplexityViolations, HighveryHighCostComplexityViolationsPrev);
                double? HighveryHighCostComplexityArtefactsEvolPerc = MathUtility.GetVariationPercent(HighveryHighCostComplexityArtefacts, HighveryHighCostComplexityArtefactsPrev);

                #endregion evolutionPercMetric

                var rowData = new List<string>() 
                    { Labels.Name
                    , Labels.Current
                    , Labels.Previous
                    , Labels.EvolutionPercent
                    , Labels.ViolationsCritical
                    , (criticalViolation.HasValue) ?  criticalViolation.Value.ToString(metricFormat):  CastReporting.Domain.Constants.No_Value
                    , (criticalViolationPrev.HasValue) ? criticalViolationPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , (criticalViolationEvolPerc.HasValue) ? TableBlock.FormatPercent(criticalViolationEvolPerc.Value): CastReporting.Domain.Constants.No_Value

                    , "  " + Labels.PerFile
                    , numCritPerFileIfNegative
                    , numCritPerFilePrevIfNegative
                    , (numCritPerFileEvolPerc.HasValue ) ? TableBlock.FormatPercent(numCritPerFileEvolPerc.Value) : CastReporting.Domain.Constants.No_Value

                    , "  " + Labels.PerkLoC
                    , (numCritPerKLOC.HasValue)? numCritPerKLOC.Value.ToString("N2") : CastReporting.Domain.Constants.No_Value
                    , (numCritPerKLOCPrev.HasValue)? numCritPerKLOCPrev.Value.ToString("N2") : CastReporting.Domain.Constants.No_Value
                    , (numCritPerKLOCEvolPerc.HasValue) ? TableBlock.FormatPercent(numCritPerKLOCEvolPerc.Value) : CastReporting.Domain.Constants.No_Value
                   
                    , Labels.ComplexObjects
                    , HighveryHighCostComplexityArtefacts.HasValue ? HighveryHighCostComplexityArtefacts.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value
                    , HighveryHighCostComplexityArtefactsPrev.HasValue ? HighveryHighCostComplexityArtefactsPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , HighveryHighCostComplexityArtefactsEvolPerc.HasValue ? TableBlock.FormatPercent(HighveryHighCostComplexityArtefactsEvolPerc.Value) : CastReporting.Domain.Constants.No_Value
                    
                    , "  " + Labels.WithViolations
                    , HighveryHighCostComplexityViolations.HasValue ? HighveryHighCostComplexityViolations.Value.ToString(metricFormat): CastReporting.Domain.Constants.No_Value
                    , HighveryHighCostComplexityViolationsPrev.HasValue ? HighveryHighCostComplexityViolationsPrev.Value.ToString(metricFormat) : CastReporting.Domain.Constants.No_Value
                    , HighveryHighCostComplexityViolationsEvolPerc.HasValue ? TableBlock.FormatPercent(HighveryHighCostComplexityViolationsEvolPerc.Value) : CastReporting.Domain.Constants.No_Value
                    };

                resultTable = new TableDefinition
                {
                    HasRowHeaders = true,
                    HasColumnHeaders = false,
                    NbRows = 6,
                    NbColumns = 4,
                    Data = rowData
                };
            }
            return resultTable;
        }
    }
}
