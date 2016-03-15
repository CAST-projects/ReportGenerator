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

namespace CastReporting.Reporting.Block.Table
{
    [Block("METRIC_TOP_ARTEFACT")]
    class MetricTopArtifact : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition back = null;
            List<string> rowData = new List<string>();
            int nbLimitTop = 0;
            List<int> bcId = new List<int>();
            int idx = -1;
            if (null == options || !options.ContainsKey("IDX") || !Int32.TryParse(options["IDX"], out idx))
            {
                idx = -1;
            }
            if (null == options || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
                if (idx >= 0 && nbLimitTop <= idx)
                    nbLimitTop = idx + 1;
            }

            bool displayHeader = (options == null || !options.ContainsKey("HEADER") || "NO" != options["HEADER"]);

            if (options != null && options.ContainsKey("PAR"))
            {
                foreach (var par in options["PAR"].Split('|'))
                {
                    int id;
                    if (Int32.TryParse(par, out id))
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


            var criticalRuleViolations = bc.CriticalRulesViolation.Where(_ => _.DetailResult != null && _.DetailResult.ViolationRatio != null).OrderByDescending(_ => _.DetailResult.ViolationRatio.FailedChecks).Take(nbLimitTop);
            if (idx >= 0)
                criticalRuleViolations = criticalRuleViolations.Skip(idx).Take(1);

            int nbRows = 0;

            foreach (var violation in criticalRuleViolations)
            {
                IEnumerable<CastReporting.Domain.MetricTopArtifact> metricTopArtefact = reportData.SnapshotExplorer.GetMetricTopArtefact(reportData.CurrentSnapshot.Href, violation.Reference.Key.ToString(), -1);

                rowData.AddRange(new string[] { "Sample Violating Artefacts for Rule '" + violation.Reference.Name + "'", "# " + nbLimitTop + " of " + metricTopArtefact.Count() });
                nbRows++;

                if (metricTopArtefact != null && metricTopArtefact.Any())
                {
                    foreach (var metric in metricTopArtefact)
                    {
                        if (nbRows < nbLimitTop)
                        {
                            rowData.AddRange(new string[] { metric.ObjectNameLocation, string.Empty });
                            nbRows += 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    rowData.AddRange(new string[] { Labels.NoItem, string.Empty });
                }
            }
              
            back = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = false,
                NbRows = nbRows,
                NbColumns = 2,
                Data = rowData
            };

            return back;
        }
    }
}
