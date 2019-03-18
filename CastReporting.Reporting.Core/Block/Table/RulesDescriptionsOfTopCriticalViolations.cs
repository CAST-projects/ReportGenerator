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
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
	[Block("RULE_NAME_DESCRIPTION_TOPCRITVIOL"), Block("METRIC_NAME_DESCRIPTION")]
	public class RulesDescriptionsOfTopCriticalViolations : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            int nbLimitTop;
            List<int> bcId = new List<int>();
            int idx;
            if (null == options || !options.ContainsKey("IDX") || !int.TryParse(options["IDX"], out idx))
            {
                idx = -1;
            }
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
                if (idx >= 0 && nbLimitTop <= idx)
                	nbLimitTop = idx + 1;
            }
            if (options != null && options.ContainsKey("PAR")) {
            	foreach (var par in options["PAR"].Split('|')) {
            		int id;
            		if (int.TryParse(par, out id)) {
            			bcId.Add(id);
            		}
            	}
            }
            if (bcId.Count == 0)
            {
            	bcId.Add(Constants.BusinessCriteria.TechnicalQualityIndex.GetHashCode());
            }
          
            ApplicationResult bc = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => bcId.Contains(_.Reference.Key));

            rowData.AddRange(new[] { "Rules Descriptions for Top Critical Violation Rules For Business Criterion " + bc?.Reference.Name, null });

            var criticalRuleViolations = bc?.CriticalRulesViolation.Where(_ => _.DetailResult?.ViolationRatio != null).OrderByDescending(_ => _.DetailResult.ViolationRatio.FailedChecks).Take(nbLimitTop);
            if (idx >= 0)
            	criticalRuleViolations = criticalRuleViolations?.Skip(idx).Take(1);

            if (criticalRuleViolations != null)
                foreach (var violation in criticalRuleViolations)
                {
                    var ruleDescription = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, violation.Reference.Key.ToString());

                    rowData.AddRange(new[]
                    {
                        Labels.RuleName, violation.Reference.Name,
                        Labels.Rationale, ruleDescription.Rationale,
                        Labels.Description, ruleDescription.Description,
                        Labels.Remediation, string.IsNullOrWhiteSpace(ruleDescription.Remediation) ? Constants.No_Value : ruleDescription.Remediation,
                        Labels.ViolationsCount, violation.DetailResult.ViolationRatio.FailedChecks.ToString(),
                        " "," "
                    });
                }

            var back = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowData.Count + 1,
                NbColumns = 2,
                Data = rowData
            };

            return back;
        }
    }
}
