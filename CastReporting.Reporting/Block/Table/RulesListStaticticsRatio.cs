/*
 *   Copyright (c) 2018 CAST
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
using CastReporting.Reporting.Languages;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("RULES_LIST_STATISTICS_RATIO")]
    public class RulesListStatisticsRatio : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> metrics = options.GetOption("METRICS").Trim().Split('|').ToList();
            bool critical;
            if (options == null || !options.ContainsKey("CRITICAL"))
            {
                critical = false;
            }
            else
            {
                critical = options.GetOption("CRITICAL").Equals("true");
            }
            bool displayCompliance = options.GetBoolOption("COMPLIANCE");
            bool sortedByCompliance = displayCompliance && options.GetOption("SORTED", "TOTAL").Equals("COMPLIANCE");

            var headers = new HeaderDefinition();
            headers.Append(Labels.RuleName);
            headers.Append(Labels.TotalViolations);
            headers.Append(Labels.AddedViolations);
            headers.Append(Labels.RemovedViolations);
            headers.Append(Labels.ComplianceScorePercent, displayCompliance);

            var dataRow = headers.CreateDataRow();
            var data = new List<string>();

            List<string> qualityRules = MetricsUtility.BuildRulesList(reportData, metrics, critical);

            List<ApplicationResult> results = sortedByCompliance ? 
                    reportData.CurrentSnapshot?.QualityRulesResults.Where(_ => qualityRules.Contains(_.Reference.Key.ToString())).OrderBy(_ => _.DetailResult.ViolationRatio.Ratio).ToList()
                    : reportData.CurrentSnapshot?.QualityRulesResults.Where(_ => qualityRules.Contains(_.Reference.Key.ToString())).OrderByDescending(_=>  _.DetailResult.ViolationRatio.FailedChecks).ToList();

            if (results?.Count > 0)
            {
                foreach (var result in results)
                {
                    var detailResult = result.DetailResult;
                    if (detailResult == null) continue;
                    dataRow.Set(Labels.RuleName, (result.Reference?.Name + " (" + result.Reference?.Key + ")" ).NAIfEmpty());
                    dataRow.Set(Labels.TotalViolations, detailResult.ViolationRatio.FailedChecks.HasValue ? detailResult.ViolationRatio?.FailedChecks.Value.ToString("N0") : Constants.No_Value);
                    dataRow.Set(Labels.AddedViolations, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty());
                    dataRow.Set(Labels.RemovedViolations, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty());
                    if (displayCompliance)
                    {
                        dataRow.Set(Labels.ComplianceScorePercent, detailResult.ViolationRatio?.Ratio.FormatPercent(false));
                    }
                    data.AddRange(dataRow);
                }
            }

            if (data.Count == 0)
            {
                dataRow.Reset();
                dataRow.Set(0, Labels.NoItem);
                data.AddRange(dataRow);
            }

            data.InsertRange(0, headers.Labels);

            return new TableDefinition
            {
                Data = data,
                HasColumnHeaders = true,
                HasRowHeaders = false,
                NbColumns = headers.Count,
                NbRows = data.Count / headers.Count
            };
        }
    }
}
