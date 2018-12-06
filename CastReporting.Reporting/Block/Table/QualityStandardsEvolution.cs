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
using System.Drawing;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("QUALITY_STANDARDS_EVOLUTION")]
    public class QualityStandardsEvolution : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string standard = options.GetOption("STD");

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(standard);
            cellidx++;
            headers.Append(Labels.TotalVulnerabilities);
            cellidx++;
            headers.Append(Labels.AddedVulnerabilities);
            cellidx++;
            headers.Append(Labels.RemovedVulnerabilities);
            cellidx++;

            var dataRow = headers.CreateDataRow();
            var data = new List<string>();

            List<ApplicationResult> results = reportData.SnapshotExplorer.GetQualityStandardsTagsResults(reportData.CurrentSnapshot.Href,standard)?.FirstOrDefault()?.ApplicationResults?.ToList();

            if (results?.Count > 0)
            {
                foreach (var result in results)
                {
                    var detailResult = result.DetailResult;
                    if (detailResult == null) continue;
                    int? nbViolations = detailResult.EvolutionSummary?.TotalViolations;
                    string stdTagName = result.Reference?.Name + " " + reportData.Application.StandardTags?.Where(_ => _.Key == result.Reference?.Name).FirstOrDefault()?.Name;
                    dataRow.Set(standard, stdTagName ?? Constants.No_Value);
                    if (nbViolations > 0)
                    {
                        cellProps.Add(new CellAttributes(cellidx, Color.Beige));

                    }
                    cellidx++;
                    dataRow.Set(Labels.TotalVulnerabilities, detailResult.EvolutionSummary?.TotalViolations.NAIfEmpty());
                    if (nbViolations > 0)
                    {
                        cellProps.Add(new CellAttributes(cellidx, Color.Beige));

                    }
                    cellidx++;
                    dataRow.Set(Labels.AddedVulnerabilities, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty());
                    if (nbViolations > 0)
                    {
                        cellProps.Add(new CellAttributes(cellidx, Color.Beige));

                    }
                    cellidx++;
                    dataRow.Set(Labels.RemovedVulnerabilities, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty());
                    if (nbViolations > 0)
                    {
                        cellProps.Add(new CellAttributes(cellidx, Color.Beige));

                    }
                    cellidx++;
                    data.AddRange(dataRow);
                }
            }

            if (data.Count == 0)
            {
                dataRow.Reset();
                dataRow.Set(0, Labels.NoRules);
                data.AddRange(dataRow);
            }

            data.InsertRange(0, headers.Labels);

            return new TableDefinition
            {
                Data = data,
                HasColumnHeaders = true,
                HasRowHeaders = false,
                NbColumns = headers.Count,
                NbRows = data.Count / headers.Count,
                CellsAttributes = cellProps
            };
        }
    }
}
