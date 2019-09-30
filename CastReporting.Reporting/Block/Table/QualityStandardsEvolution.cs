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
using Cast.Util.Log;
using Cast.Util.Version;
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
            bool detail = options.GetOption("MORE", "false").ToLower().Equals("true");
            bool vulnerability = options.GetOption("LBL", "vulnerabilities").ToLower().Equals("vulnerabilities");
            string lbltotal = vulnerability ? Labels.TotalVulnerabilities : Labels.TotalViolations;
            string lbladded = vulnerability ? Labels.AddedVulnerabilities : Labels.AddedViolations;
            string lblremoved = vulnerability ? Labels.RemovedVulnerabilities : Labels.RemovedViolations;

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(standard);
            cellidx++;
            headers.Append(lbltotal);
            cellidx++;
            headers.Append(lbladded);
            cellidx++;
            headers.Append(lblremoved);
            cellidx++;

            var data = new List<string>();

            if (!VersionUtil.Is111Compatible(reportData.ServerVersion) && detail)
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.11 at least for component QUALITY_STANDARDS_EVOLUTION and MORE option");
                var dataRow = headers.CreateDataRow();
                dataRow.Set(standard, Labels.NoData);
                dataRow.Set(lbltotal, string.Empty);
                dataRow.Set(lbladded, string.Empty);
                dataRow.Set(lblremoved, string.Empty);
                data.AddRange(dataRow);
                data.InsertRange(0, headers.Labels);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 4,
                    Data = data
                };
            }

            List<ApplicationResult> results = reportData.SnapshotExplorer.GetQualityStandardsTagsResults(reportData.CurrentSnapshot.Href,standard)?.FirstOrDefault()?.ApplicationResults?.ToList();

            if (results?.Count > 0)
            {
                foreach (var result in results)
                {
                    var dataRow = headers.CreateDataRow();
                    var detailResult = result.DetailResult;
                    if (detailResult == null) continue;
                    int? nbViolations = detailResult.EvolutionSummary?.TotalViolations;
                    // usefull when the STD is a tag. when STD is a category it is not in the standardTags list for application, so only STD name is displayed
                    string stdTagName = result.Reference?.Name + " " + reportData.Application.StandardTags?.Where(_ => _.Key == result.Reference?.Name).FirstOrDefault()?.Name;
                    dataRow.Set(standard, stdTagName);
                    FormatHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
                    cellidx++;
                    dataRow.Set(lbltotal, detailResult.EvolutionSummary?.TotalViolations.NAIfEmpty("N0"));
                    FormatHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
                    cellidx++;
                    dataRow.Set(lbladded, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty("N0"));
                    FormatHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
                    cellidx++;
                    dataRow.Set(lblremoved, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty("N0"));
                    FormatHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
                    cellidx++;
                    data.AddRange(dataRow);

                    // add lines for all sub tags if detail version
                    if (!detail) continue;
                    {
                        List<ApplicationResult> stdresults = reportData.SnapshotExplorer.GetQualityStandardsTagsResults(reportData.CurrentSnapshot.Href, result.Reference?.Name)?.FirstOrDefault()?.ApplicationResults?.ToList();
                        if (!(stdresults?.Count > 0)) continue;
                        foreach (var stdres in stdresults)
                        {
                            var stddataRow = headers.CreateDataRow();

                            var detailStdResult = stdres.DetailResult;
                            if (detailStdResult == null) continue;
                            int? nbStdViolations = detailStdResult.EvolutionSummary?.TotalViolations;
                            string stdresTagName = stdres.Reference?.Name + " " + reportData.Application.StandardTags?.Where(_ => _.Key == stdres.Reference?.Name).FirstOrDefault()?.Name;
                            stddataRow.Set(standard, "    " + stdresTagName);
                            FormatHelper.AddGrayOrBold(false, cellProps, cellidx, nbStdViolations);
                            cellidx++;
                            stddataRow.Set(lbltotal, detailStdResult.EvolutionSummary?.TotalViolations.NAIfEmpty("N0"));
                            FormatHelper.AddGrayOrBold(false, cellProps, cellidx, nbStdViolations);
                            cellidx++;
                            stddataRow.Set(lbladded, detailStdResult.EvolutionSummary?.AddedViolations.NAIfEmpty("N0"));
                            FormatHelper.AddGrayOrBold(false, cellProps, cellidx, nbStdViolations);
                            cellidx++;
                            stddataRow.Set(lblremoved, detailStdResult.EvolutionSummary?.RemovedViolations.NAIfEmpty("N0"));
                            FormatHelper.AddGrayOrBold(false, cellProps, cellidx, nbStdViolations);
                            cellidx++;
                            data.AddRange(stddataRow);
                        }
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(standard, Labels.NoRules);
                dataRow.Set(lbltotal, string.Empty);
                dataRow.Set(lbladded, string.Empty);
                dataRow.Set(lblremoved, string.Empty);
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
