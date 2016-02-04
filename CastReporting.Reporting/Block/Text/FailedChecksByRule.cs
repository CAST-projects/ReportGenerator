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

namespace CastReporting.Reporting.Block.Text
{

    [Block("RULE_FAILED_CHECKS")]
    class FailedChecksByRule : TextBlock
    {
        #region METHODS
        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string strRuleId = (options != null && options.ContainsKey("RULID")) ? options["RULID"] : "7126";

            if (null != reportData && null != reportData.CurrentSnapshot) {
                var rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, strRuleId);
                var currentviolation = reportData.RuleExplorer.GetRulesViolations(reportData.CurrentSnapshot.Href, strRuleId).FirstOrDefault();

                Int32? failedChecks = null;

                if (currentviolation != null && currentviolation.ApplicationResults.Any()) {
                    failedChecks = currentviolation.ApplicationResults[0].DetailResult.ViolationRatio.FailedChecks;
                }

                return (failedChecks != null && failedChecks.HasValue) ? failedChecks.Value.ToString("N0") : Constants.No_Value;
            }
            return CastReporting.Domain.Constants.No_Value;
        }
        #endregion METHODS
    }
}


