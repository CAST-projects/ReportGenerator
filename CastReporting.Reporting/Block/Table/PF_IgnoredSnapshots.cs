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
using System.Data;

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_IGNORED_SNAPSHOTS")]
    class PF_IgnoredSnapshots : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;
            List<string> rowData = new List<string>();
            int nbRows = 0;

            rowData.Add("Ignored Snapshots");

            if (reportData.IgnoresSnapshots.Count() == 0)
            {
                rowData.Add("No Ignored Snapshots");
                nbRows++;
            }
            else
            {
                string[] DistinctArray = reportData.IgnoresSnapshots.Distinct().ToArray();
                for (int i = 0; i < DistinctArray.Count(); i++)
                {
                    rowData.Add(DistinctArray[i].ToString());
                }
                nbRows = DistinctArray.Count();
            }

            resultTable = new TableDefinition
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
