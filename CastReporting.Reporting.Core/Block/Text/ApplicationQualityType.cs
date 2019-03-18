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

namespace CastReporting.Reporting.Block.Text
{
    [Block("APPLICATION_QUALITY_TYPE")]
    public class ApplicationQualityType : TextBlock
    {


        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (reportData?.CurrentSnapshot == null) return Constants.No_Value;
            double? tqi = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(reportData.CurrentSnapshot, Constants.BusinessCriteria.TechnicalQualityIndex, true);

            return tqi.HasValue ? GetApplicationQualification(reportData, tqi.Value) : Constants.No_Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetApplicationQualification(ReportData reportData, double value)
        {
            if (value < reportData.Parameter.ApplicationQualityVeryLow)
                return Labels.QualityVeryLow;

            if (value < reportData.Parameter.ApplicationQualityLow)
                return Labels.QualityLow;

            if (value < reportData.Parameter.ApplicationQualityMedium)
                return Labels.QualityMedium;

            return value < reportData.Parameter.ApplicationQualityGood ? Labels.QualityGood : Labels.QualityVeryGood;
        }
        
        #endregion METHODS

        
    }
}
