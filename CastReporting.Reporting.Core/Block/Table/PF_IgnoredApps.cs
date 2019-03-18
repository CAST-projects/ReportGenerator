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

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_IGNORED_APPLICATIONS")]
    // ReSharper disable once InconsistentNaming
    public class PF_IgnoredApps : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            int nbRows = 0;
            rowData.Add("Ignored Applications");

            if (reportData.IgnoresApplications.Length == 0)
            {
                rowData.Add("No Ignored Applications");
                nbRows++;
            }
            else
            {
                string[] _distinctArray = reportData.IgnoresApplications.Distinct().ToArray();
                rowData.AddRange(_distinctArray.Select(_ => _.ToString()));
                nbRows = _distinctArray.Length;
            }
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 1,
                Data = rowData
            };

            return resultTable;
        }
    }
}
