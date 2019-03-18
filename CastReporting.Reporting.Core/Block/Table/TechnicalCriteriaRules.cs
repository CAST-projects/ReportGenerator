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

using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TECHNICAL_CRITERIA_RULES")]
    public class TechnicalCriteriaRules : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;

            #region Options
            int nblimit;
            if (null == options || !options.ContainsKey("CNT") || !int.TryParse(options["CNT"], out nblimit)) {
                nblimit = reportData.Parameter.NbResultDefault;
            }
            int techcriteriaId;
            if (null == options || !options.ContainsKey("TCID") || !int.TryParse(options["TCID"], out techcriteriaId)) {
                throw new ArgumentException("Impossible to build TECHNICAL_CRITERIA_RULES : Need technical criterion id.");
            }
            int bizCriteriaId;
            if (null == options || !options.ContainsKey("BZID") || !int.TryParse(options["BZID"], out bizCriteriaId)) {
                throw new ArgumentException("Impossible to build TECHNICAL_CRITERIA_RULES : Need business criterion id.");
            }
            #endregion Options

            List<string> rowData = new List<string>();
			rowData.AddRange(new[] {
				Labels.RuleName,
				Labels.Description,
				Labels.ViolationsCount
			});

            var businessCriteria = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == bizCriteriaId);

            var keys = businessCriteria?.CriticalRulesViolation.Select(_ => _.Reference.Key).ToList();
            keys?.AddRange(businessCriteria.NonCriticalRulesViolation.Select(_ => _.Reference.Key));

            // ReSharper disable once PossibleNullReferenceException
            var technicalCriteria = (keys != null) ? reportData.CurrentSnapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == techcriteriaId && keys.Contains(_.Reference.Key)) : null;

            if (technicalCriteria?.RulesViolation != null)
            {
                var results = technicalCriteria.RulesViolation
                                               .OrderByDescending(_ => _.DetailResult.ViolationRatio.FailedChecks)
                                               .Take(nblimit);
                
                foreach (var violation in results) {
                    var ruleDescription = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, violation.Reference.Key.ToString());

                    rowData.AddRange(
                            new[] {
								violation.Reference.Name
	                            , violation.DetailResult.ViolationRatio.FailedChecks > 0 ? ruleDescription.Description : string.Empty
	                            , violation.DetailResult.ViolationRatio.FailedChecks > 0 ? violation.DetailResult.ViolationRatio.FailedChecks.ToString() : string.Empty
                            });
					
					rowCount++;
                }
            }

            if (rowCount == 0) {
				rowData.AddRange(new[] {
					Labels.NoItem,
					string.Empty,
					string.Empty
				});
                rowCount = 1;
            }

			TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowCount + 1,
                NbColumns = 3,
                Data = rowData,
            };

            return resultTable;
        }
    }
}
