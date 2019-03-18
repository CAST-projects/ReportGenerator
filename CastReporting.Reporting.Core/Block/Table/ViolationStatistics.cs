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
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("VIOLATION_STATISTICS")]
    public class ViolationStatistics : TableBlock
    {
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (reportData?.CurrentSnapshot == null) return null;

            double? criticalViolation = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesNumber);
            double? numCritPerFile = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerFileNumber);
            double? _numCritPerKloc = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, Constants.SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);

            double? veryHighCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,Constants.
                    QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                Constants.DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_VeryHigh.GetHashCode());
                
            double? highCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                Constants.QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.GetHashCode(),
                Constants.DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_High.GetHashCode());

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

            string numCritPerFileIfNegative;
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- special case
            if (numCritPerFile == -1)
                numCritPerFileIfNegative = Constants.No_Value;
            else
                numCritPerFileIfNegative = numCritPerFile?.ToString(metricFormatPrecision) ?? Constants.No_Value;
                
            var rowData = new List<string>() 
            { 
                Labels.Name
                , Labels.Value
                    
                , Labels.ViolationsCritical
                , criticalViolation?.ToString(metricFormat) ?? Constants.No_Value

                , "  " + Labels.PerFile
                , numCritPerFileIfNegative

                , "  " + Labels.PerkLoC
                , _numCritPerKloc?.ToString(metricFormatPrecision) ?? Constants.No_Value

                , Labels.ComplexObjects
                , nbComplexityArtefacts?.ToString(metricFormat) ?? Constants.No_Value
                
                , "  " + Labels.WithViolations
                , nbComplexityArtefactsViolation?.ToString(metricFormat) ?? Constants.No_Value
            };
                
            var resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = 6,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
