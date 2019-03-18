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

namespace CastReporting.Reporting.Block.Text
{
    [Block("APPLICATION_SIZE_TYPE")]
    public class ApplicationSizeType : TextBlock
    {


        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (reportData?.CurrentSnapshot == null) return Domain.Constants.No_Value;
            double? codeLineNumber = MeasureUtility.GetCodeLineNumber(reportData.CurrentSnapshot);

            return codeLineNumber.HasValue ? GetApplicationQualification(reportData, codeLineNumber.Value) : Domain.Constants.No_Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetApplicationQualification(ReportData reportData, double value)
        {
            if (value < reportData.Parameter.ApplicationSizeLimitSupSmall)
				return Labels.SizeS;
            if (value < reportData.Parameter.ApplicationSizeLimitSupMedium)
                return Labels.SizeM;
            return value < reportData.Parameter.ApplicationSizeLimitSupLarge ? Labels.SizeL : Labels.SizeXL;
        }

        #endregion METHODS

    }
}
