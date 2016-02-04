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

namespace CastReporting.Reporting.Block.Text
{
    [Block("APPLICATION_QUALITY_TYPE")]
    class ApplicationQualityType : TextBlock
    {

        
        #region METHODS
        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (null != reportData &&
                null != reportData.CurrentSnapshot)
            {
                Double? tqi = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(reportData.CurrentSnapshot, Constants.BusinessCriteria.TechnicalQualityIndex);

                if (tqi.HasValue) return GetApplicationQualification(reportData, tqi.Value);
            }
            return CastReporting.Domain.Constants.No_Value;

        }

        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="value"></param>
        /// <param name="qualifyType"></param>
        /// <returns></returns>
        private static String GetApplicationQualification(ReportData reportData, double value)
        {
          
            if (value < reportData.Parameter.ApplicationQualityVeryLow)
                return Labels.QualityVeryLow;

            else if (value < reportData.Parameter.ApplicationQualityLow)
                return Labels.QualityLow;

            else if (value < reportData.Parameter.ApplicationQualityMedium)
                return Labels.QualityMedium;

            else if (value < reportData.Parameter.ApplicationQualityGood)
                return Labels.QualityGood;

            else
                return Labels.QualityVeryGood;
        }
        
        #endregion METHODS

        
    }
}
