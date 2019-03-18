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
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("VIOLATION_SUMMARY")]
    public class ViolationSummary : TableBlock
    {
        private static ResultDetail GetModuleResult(ApplicationResult ar, Module module)
        {
            ResultDetail detailResult = null;
            if (module == null) return null;
            var modResult = ar.ModulesResult.FirstOrDefault(mr => mr != null && mr.Module.Id == module.Id);
            if (modResult != null)
                detailResult = modResult.DetailResult;
            return detailResult;
        }

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimit = options.GetIntOption("COUNT", reportData.Parameter.NbResultDefault); // Max number of rows; -1 correspond to all results
            bool perModule = options.GetBoolOption("MODULES"); // module or application mode
            bool showGrades = options.GetBoolOption("GRADE", true); // show/hide grades
            bool showCritical = options.GetBoolOption("CRITICAL", true); // show/hide critical rules
            bool showNonCritical = options.GetBoolOption("NONCRITICAL"); // show/hide critical rules
            bool showAddedRemoved = options.GetBoolOption("ADDEDREMOVED", true); // show/hide added/removed
            bool showTotal = options.GetBoolOption("TOTAL", true); // show/hide total
            bool showFailedChecks = options.GetBoolOption("FAILED"); // show/hide failed checks
            bool showSuccessfulChecks = options.GetBoolOption("SUCCESSFUL"); // show/hide successfull checks
            bool showCompliance = options.GetBoolOption("COMPLIANCE"); // show/hide compliance ration

            var headers = new HeaderDefinition();
            headers.Append(Labels.ModuleName, perModule);
            headers.Append(Labels.RuleName);
            headers.Append(Labels.Grade, showGrades);
            headers.Append(Labels.ViolationsCount, showFailedChecks);
            headers.Append(Labels.TotalOk, showSuccessfulChecks);
            headers.Append(Labels.TotalChecks, showTotal);
            headers.Append(Labels.Compliance, showCompliance);
            headers.Append(Labels.ViolationsAdded, showAddedRemoved);
            headers.Append(Labels.ViolationsRemoved, showAddedRemoved);
            headers.Append(Labels.Critical);

            var dataRow = headers.CreateDataRow();
            var data = new List<string>();

            int nbRow = 0;

            if (reportData.CurrentSnapshot != null) {
                Dictionary<int, RuleDetails> targetRules =
                    reportData.RuleExplorer
                    .GetRulesDetails(reportData.CurrentSnapshot.DomainId, Constants.BusinessCriteria.TechnicalQualityIndex.GetHashCode(), reportData.CurrentSnapshot.Id)
                    .Where(rd => rd.Key.HasValue && ((showCritical && rd.Critical) || (showNonCritical && rd.Critical == false)))
                    .ToDictionary(rd => rd.Key.Value);

                var sourceResults = reportData.CurrentSnapshot.QualityRulesResults.Where(qr => targetRules.ContainsKey(qr.Reference.Key)).ToList();

                var modules = (perModule ? reportData.CurrentSnapshot.Modules : new List<Module>(new Module[] { null }).AsEnumerable()).OrderBy(m => (m == null ? string.Empty : m.Name));

                foreach (var module in modules) {
                    if (perModule) {
                        dataRow.Set(Labels.ModuleName, module.Name.NAIfEmpty());
                    }

                    var query = sourceResults.Select(ar => new {ar.Reference, AppDetailResult = ar.DetailResult, ModDetailResult = GetModuleResult(ar, module) });

                    foreach (var result in query)
                    {
                        if ((nbLimit != -1) && (nbRow >= nbLimit)) continue;
                        var detailResult = perModule ? result.ModDetailResult : result.AppDetailResult;
                        if (detailResult != null && detailResult.Grade > 0)
                        {
                            dataRow.Set(Labels.RuleName, result.Reference?.Name.NAIfEmpty());
                            if (showGrades)
                            {
                                dataRow.Set(Labels.Grade, detailResult.Grade?.ToString("N2"));
                            }
                            if (showFailedChecks)
                            {
                                dataRow.Set(Labels.ViolationsCount, (bool) detailResult.ViolationRatio?.FailedChecks.HasValue ? detailResult.ViolationRatio?.FailedChecks.Value.ToString("N0") : Constants.No_Value);
                            }
                            if (showSuccessfulChecks)
                            {
                                dataRow.Set(Labels.TotalOk, (bool) detailResult.ViolationRatio?.SuccessfulChecks.HasValue ? detailResult.ViolationRatio?.SuccessfulChecks.Value.ToString("N0") : Constants.No_Value);
                            }
                            if (showTotal)
                            {
                                dataRow.Set(Labels.TotalChecks, (bool) detailResult.ViolationRatio?.TotalChecks.HasValue ? detailResult.ViolationRatio?.TotalChecks.Value.ToString("N0") : Constants.No_Value);
                            }
                            if (showCompliance)
                            {
                                dataRow.Set(Labels.Compliance, detailResult.ViolationRatio?.Ratio.FormatPercent(false));
                            }
                            if (showAddedRemoved)
                            {
                                dataRow.Set(Labels.ViolationsAdded, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty());
                                dataRow.Set(Labels.ViolationsRemoved, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty());
                            }
                            var ruleId = result.Reference?.Key;
                            dataRow.Set(Labels.Critical, (ruleId.HasValue && targetRules.ContainsKey(ruleId.Value) && targetRules[ruleId.Value].Critical) ? "X" : "");

                            data.AddRange(dataRow);
                        }
                        nbRow++;
                    }
                }
            }

            if (data.Count == 0) {
                dataRow.Reset();
                dataRow.Set(0, Labels.NoItem);
                data.AddRange(dataRow);
            }

            data.InsertRange(0, headers.Labels);

            return new TableDefinition {
                Data = data,
                HasColumnHeaders = true,
                HasRowHeaders = false,
                NbColumns = headers.Count,
                NbRows = data.Count / headers.Count
            };
        }
    }
}
