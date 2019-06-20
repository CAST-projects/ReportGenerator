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

// ReSharper disable InconsistentNaming

namespace CastReporting.Domain
{
    public class Constants
    {
        #region CONSTANTS
        public enum SizingInformations
        {
            CommentLineNumber = 10107,
            CommentedOutCodeLinesNumber = 10109,
            CodeLineNumber = 10151,
            ArtifactNumber = 10152,
            FileNumber = 10154,
            ClassNumber = 10155,
            SQLArtifactNumber = 10158,
            WebPageNumber = 10159,
            MethodNumber = 10161,
            TableNumber = 10163,
            PackageNumber = 10166,
            BackfiredIFPUGFunctionPoints = 10201,
            UnadjustedDataFunctions = 10203,
            UnadjustedTransactionalFunctions = 10204,
            AutomatedIFPUGFunctionPointsEstimation = 10202,
            AddedDataFunctionPoints = 10301,
            AddedTransactionalFunctionPoints = 10302,
            AddedFunctionPoints = 10300,
            DeletedDataFunctionPoints = 10321,
            DeletedTransactionalFunctionPoints = 10322,
            DeletedFunctionPoints = 10320,
            DecisionPointsNumber = 10506,
            CriticalQualityRulesWithViolationsNumber = 67010,
            ViolationsToCriticalQualityRulesNumber = 67011,
            ViolationsToCriticalQualityRulesPerFileNumber = 67012,
            ViolationsToCriticalQualityRulesPerKLOCNumber = 67013,
            CriticalQualityRulesWithViolationsNewModifiedCodeNumber = 67014,
            ViolationsToCriticalQualityRulesNewModifiedCodeNumber = 67015,
            TechnicalDebt = 68001,
            TechnicalDebtDensity = 68002,
            AddedViolationsTechnicalDebt =  68901,
            RemovedViolationsTechnicalDebt = 68902,
            ModifiedDataFunctionPoints = 10311,
            ModifiedTransactionalFunctionPoints = 10312,
            ModifiedFunctionPoints = 10310

        }
        
        
        public enum BusinessCriteria
        {
            Transferability = 60011,
            Changeability = 60012,
            Robustness = 60013,
            Performance = 60014,
            Security = 60016,
            TechnicalQualityIndex = 60017,

            ProgrammingPractices = 66031,
            ArchitecturalDesign = 66032,
            Documentation = 66033,
            SEIMaintainability = 60015
        }

        public enum Qualify
        {
            ApplicationSize = 1,
            QualityType = 2
        }
        public enum RulesViolation
        {
            All = 0,
            NonCriticalRulesViolation = 1,
            CriticalRulesViolation = 2
        }
        public enum QualityDistribution
        {
            CyclomaticComplexityDistribution = 65501,
            FourGLComplexityDistribution = 65601,
            ClassComplexityDistributionWMC = 66015,
            OOComplexityDistribution = 65701,
            SQLComplexityDistribution = 65801,
            CouplingDistribution = 65350,
            ClassFanOutDistribution = 66020,
            ClassFanInDistribution = 66021,
            SizeDistribution = 65105,
            ReuseByCallDistribution = 66010,
            CostComplexityDistribution = 67001,
            DistributionOfViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity = 67020,
            DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity = 67030
        }

        public enum CostComplexity
        {
            CostComplexityArtifacts_Low =  67005,
            CostComplexityArtifacts_Average =  67004,
            CostComplexityArtifacts_High =  67003,
            CostComplexityArtifacts_VeryHigh = 67002
        }
        public enum ViolationsToCriticalDiagnosticBasedMetricsPerCostComplexity
        {
            ComplexityViolations_VeryHigh = 67021, 
            ComplexityViolations_HighCost = 67022,  
            ComplexityViolations_Average = 67023, 
            ComplexityViolations_LowCost = 67024
        }
        public enum DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity
        {
            CostComplexityDefects_Low = 67034,
            CostComplexityDefects_Average = 67033,
            CostComplexityDefects_High = 67032,
            CostComplexityDefects_VeryHigh = 67031
        }

        public enum CyclomaticComplexity
        {
            ComplexityArtifacts_Low = 65502,
            ComplexityArtifacts_Moderate = 65503,
            ComplexityArtifacts_High = 65504,
            ComplexityArtifacts_VeryHigh = 65505
        }
        public enum FourGLComplexity
        {
            FourGLComplexityForms_Low = 65602,
            FourGLComplexityForms_Moderate = 65603,
            FourGLComplexityForms_High = 65604,
            FourGLComplexityForms_Very = 65605
        }
        public enum ClassComplexity
        {
            Complexityclasses_Low = 66019,
            Complexityclasses_Moderate = 66018,
            Complexityclasses_High = 66017,
            Complexityclasses_VeryHigh = 66016
        }
        public enum OOComplexity
        {
            OOComplexityClass_Low = 65702,
            OOComplexityClass_Moderate = 65703,
            OOComplexityClass_High = 65704,
            OOComplexityClass_VeryHigh = 65705
        }
        public enum SQLComplexityDistribution
        {
            SQLComplexityArtifacts_Low = 65802,
            SQLComplexityArtifacts_Moderate = 65803,
            SQLComplexityArtifacts_High = 65804,
            SQLComplexityArtifacts_VeryHigh = 65805
        }
        public enum CouplingArtifacts
        {
            CouplingArtifacts_Low = 65301,
            CouplingArtifacts_Average = 65302,
            CouplingArtifacts_High = 65303,
            CouplingArtifacts_VeryHigh = 65304
        }
        public enum ClassFanOut
        {
            FanOutclasses_Low = 66025,
            FanOutclasses_Moderate = 66024,
            FanOutclasses_High = 66023,
            FanOutclasses_VeryHigh = 66022
        }
        public enum ClassFanIn
        {
            FanInclasses_Low = 66029,
            FanInclasses_Moderate = 66028,
            FanInclasses_High = 66027,
            FanInclasses_VeryHigh = 66026
        }
        public enum SizeArtifacts
        {
            SizeArtifacts_Small = 65101,
            SizeArtifacts_Average = 65102,
            SizeArtifacts_Large = 65103,
            SizeArtifacts_VeryLarge = 65104
        }
        public enum ReuseByCall
        {
            ReusebyCall_Low = 66014,
            ReusebyCall_Average = 66013,
            ReusebyCall_High = 66012,
            ReusebyCall_VeryHigh = 66011
        }

        public const string No_Value = "n/a";
		public const string No_Data = "-";
        public const string Zero = "0";
        #endregion CONSTANTS

        #region ATTRIBUTES
        private static readonly object _lock = new object();
        private static Constants _instance;
        #endregion ATTRIBUTES

        #region CONSTRUCTORS
        private Constants()
        {
        }
        #endregion CONSTRUCTORS

        #region PROPERTIES
        public static Constants Instance
        {
            get
            {
                if (null != _instance) return _instance;
                lock (_lock)
                {
                    if (null == _instance)
                    {
                        // ReSharper disable once PossibleMultipleWriteAccessInDoubleCheckLocking
                        _instance = new Constants();
                    }
                }
                return _instance;
            }
        }

        #endregion PROPERTIES
    }

}
