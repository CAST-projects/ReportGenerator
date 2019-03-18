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
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("METRIC_TOP_ARTEFACT")]
    public class MetricTopArtifact : TableBlock
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

            if (options != null && options.ContainsKey("PAR"))
            {
                foreach (var par in options["PAR"].Split('|'))
                {
                    int id;
                    if (int.TryParse(par, out id))
                    {
                        bcId.Add(id);
                    }
                }
            }
            if (bcId.Count == 0)
            {
                bcId.Add(Constants.BusinessCriteria.TechnicalQualityIndex.GetHashCode());
            }

            ApplicationResult bc = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => bcId.Contains(_.Reference.Key));


            var criticalRuleViolations = bc?.CriticalRulesViolation.Where(_ => _.DetailResult?.ViolationRatio != null).OrderByDescending(_ => _.DetailResult.ViolationRatio.FailedChecks).Take(nbLimitTop);
            if (idx >= 0)
                criticalRuleViolations = criticalRuleViolations?.Skip(idx).Take(1);

            int nbRows = 0;

            if (criticalRuleViolations != null)
                foreach (var violation in criticalRuleViolations)
                {
                    IEnumerable<Domain.MetricTopArtifact> metricTopArtefact = reportData.SnapshotExplorer.GetMetricTopArtefact(reportData.CurrentSnapshot.Href, violation.Reference.Key.ToString(), -1)?.ToList();

                    int nbArtefactsDisp = 0;
                    int nbArtefactsCount = 0;
                    if (metricTopArtefact != null && metricTopArtefact.Any())
                    {
                        nbArtefactsCount = metricTopArtefact.Count();
                        nbArtefactsDisp = Math.Min(nbLimitTop, nbArtefactsCount);
                    }
                    rowData.AddRange(new[] { "Sample Violating Artefacts for Rule '" + violation.Reference.Name + "'", "# " + nbArtefactsDisp + " of " + nbArtefactsCount });
                    nbRows++;

                    if (metricTopArtefact != null &&  metricTopArtefact.Any() && nbArtefactsDisp > 0)
                    {
                        foreach (var metric in metricTopArtefact)
                        {
                            if (nbArtefactsDisp > 0)
                            {
                                rowData.AddRange(new[] { metric.ObjectNameLocation, string.Empty });
                                nbRows++;
                                nbArtefactsDisp--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        rowData.AddRange(new[] { Labels.NoItem, string.Empty });
                        nbRows++;
                    }
                }

            if (nbRows == 0) {		
                rowData.AddRange(new[] { Labels.NoItem, string.Empty });
                nbRows++;
            }
            
            var back = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows - 1,
                NbColumns = 2,
                Data = rowData
            };

            return back;
        }
    }
}
