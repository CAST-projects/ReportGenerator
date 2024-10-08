﻿/*
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
using System.Drawing;
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
    [Block("LIST_TAGS_DOC_BYCAT")]
    public class ListTagsDocByCategory : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> categories = options.GetOption("CAT").Trim().Split('|').ToList();

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(Labels.Standards);
            cellidx++;
            headers.Append(Labels.Definition);
            cellidx++;
            headers.Append(Labels.Applicability);
            cellidx++;

            var data = new List<string>();

            if (!VersionUtil.Is112Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.12 at least for component LIST_TAGS_DOC_BYCAT");
                var dataRow = headers.CreateDataRow();
                dataRow.Set(Labels.Standards, Labels.NoData);
                dataRow.Set(Labels.Definition, string.Empty);
                dataRow.Set(Labels.Applicability, string.Empty);
                data.AddRange(dataRow);
                data.InsertRange(0, headers.Labels);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 3,
                    Data = data
                };
            }

            bool moreThanOne = categories.Count > 1;
            if (categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    if (moreThanOne)
                    {
                        var dataRowCat = headers.CreateDataRow();
                        dataRowCat.Set(Labels.Standards, category);
                        FormatHelper.AddGrayAndBold(cellProps, cellidx);
                        cellidx++;
                        dataRowCat.Set(Labels.Definition, "");
                        FormatHelper.AddGrayAndBold(cellProps, cellidx);
                        cellidx++;
                        dataRowCat.Set(Labels.Applicability, "");
                        FormatHelper.AddGrayAndBold(cellProps, cellidx);
                        cellidx++;
                        data.AddRange(dataRowCat);
                    }
                    List<StandardTag> tagsDoc = reportData.RuleExplorer.GetQualityStandardTagsApplicabilityByCategory(reportData.Application.DomainId, category).ToList();
                    foreach (var doc in tagsDoc)
                    {
                        var dataRow = headers.CreateDataRow();
                        bool isApplicable = doc.Applicable.Equals("true");
                        dataRow.Set(Labels.Standards, doc.Key);
                        FormatHelper.AddColorsIfCondition(isApplicable, cellProps, cellidx, Color.MintCream, Color.BlanchedAlmond);
                        cellidx++;
                        dataRow.Set(Labels.Definition, doc.Name);
                        FormatHelper.AddColorsIfCondition(isApplicable, cellProps, cellidx, Color.MintCream, Color.BlanchedAlmond);
                        cellidx++;
                        dataRow.Set(Labels.Applicability, isApplicable ? Labels.Applicable : Labels.NotApplicable);
                        FormatHelper.AddColorsIfCondition(isApplicable, cellProps, cellidx, Color.MintCream, Color.BlanchedAlmond);
                        cellidx++;
                        data.AddRange(dataRow);
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(Labels.Standards, Labels.NoRules);
                dataRow.Set(Labels.Definition, string.Empty);
                dataRow.Set(Labels.Applicability, string.Empty);
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
