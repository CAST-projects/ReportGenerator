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
    [Block("RULE_NAME_DESCRIPTION_TOPCRITVIOL")]
    class RulesDescriptionsOfTopCriticalViolations : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition back = null;
            List<string> rowData = new List<string>();
            int nbLimitTop = 0;
            int bcId = 0;
            if (null == options || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
            }
            if (null == options || !options.ContainsKey("PAR") || !Int32.TryParse(options["PAR"], out bcId))
            {
                bcId = Constants.BusinessCriteria.TechnicalQualityIndex.GetHashCode();
            }
          
            ApplicationResult bc = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == bcId).FirstOrDefault();

            rowData.AddRange(new string[] { "Rules Descriptions for Top Critical Violation Rules For Business Criterion " + bc.Reference.Name, null, });

            var criticalRuleViolations = bc.CriticalRulesViolation.Where(_ => _.DetailResult != null && _.DetailResult.ViolationRatio != null).OrderByDescending(_ => _.DetailResult.ViolationRatio.FailedChecks).Take(nbLimitTop).ToList();

            foreach (var violation in criticalRuleViolations)
            {
                RuleDescription ruleDescription = null;

                ruleDescription = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, violation.Reference.Key.ToString());

                rowData.AddRange(new string[]
                                    {
                                        "Rule Name", violation.Reference.Name,
                                        "Rational", ruleDescription.Rationale,
                                        "Description", ruleDescription.Description,
                                        "Remediation", string.IsNullOrWhiteSpace(ruleDescription.Remediation) ? Constants.No_Value : ruleDescription.Remediation,
                                        "Violations #", violation.DetailResult.ViolationRatio.FailedChecks.ToString("N0"),
                                            " "," "
                                    });
            }

            back = new TableDefinition
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
