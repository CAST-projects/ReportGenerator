using CastReporting.Domain;
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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    /// <summary>
    /// 
    /// </summary>
    [Block("RULE_NAME_DESCRIPTION")]
    public class RuleNameAndDescription : TableBlock
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string strRuleId = (options != null && options.ContainsKey("RULID")) ? options["RULID"] : null;
   
            List<string> rowData = new List<string>();
       
            var rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, strRuleId);
            var currentviolation = reportData.RuleExplorer.GetRulesViolations(reportData.CurrentSnapshot.Href, strRuleId).FirstOrDefault();
            int? failedChecks = null;

            if (currentviolation != null && currentviolation.ApplicationResults.Any()) {
                failedChecks = currentviolation.ApplicationResults[0].DetailResult.ViolationRatio.FailedChecks;               
            }

            if (rule != null) {
                rowData.AddRange(new[] {
                                rule.Name, null,
					Labels.Rationale, string.IsNullOrWhiteSpace(rule.Rationale) ? Constants.No_Value : rule.Rationale,
					Labels.Description, rule.Description,
					Labels.Remediation, string.IsNullOrWhiteSpace(rule.Remediation) ? Constants.No_Value : rule.Remediation,
					Labels.ViolationsCount, failedChecks?.ToString("N0") ?? Constants.No_Value,
                            });
            }
                


			TableDefinition back = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 5,
                NbColumns = 2,
                Data = rowData
            };

            return back;
        }
    }
}
