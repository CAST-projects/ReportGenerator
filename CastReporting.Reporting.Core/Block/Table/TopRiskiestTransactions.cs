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
    [Block("TOP_RISKIEST_TRANSACTIONS")]
    public class TopRiskiestTransactions : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            const string metricFormat = "N0";
            int nbLimitTop;
            int nbRows = 0;
            List<string> rowData = new List<string>(new[] { Labels.TransactionEP, Labels.TRI });

            // Default Options
            int businessCriteria = 0;
            if (options != null &&
                options.ContainsKey("SRC"))
            {
                var source = options["SRC"];
                switch (source)
                {
                    case "PERF": { businessCriteria = (int)Domain.Constants.BusinessCriteria.Performance; } break;
                    case "ROB": { businessCriteria = (int)Domain.Constants.BusinessCriteria.Robustness; } break;
                    case "SEC": { businessCriteria = (int)Domain.Constants.BusinessCriteria.Security; } break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (options == null ||
                !options.ContainsKey("COUNT") ||
                !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
            }

            var bc = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == businessCriteria);

            if (bc != null)
            {
                bc.Transactions = reportData.SnapshotExplorer.GetTransactions(reportData.CurrentSnapshot.Href, bc.Reference.Key.ToString(), nbLimitTop)?.ToList();
                if (bc.Transactions !=null && bc.Transactions.Any())
                {
                    foreach (var transaction in bc.Transactions)
                    {
                        rowData.Add(transaction.Name);
                        rowData.Add(transaction.TransactionRiskIndex.ToString(metricFormat));
                        nbRows += 1;
                    }
                }
                else
                {
                    rowData.AddRange(new[] { Labels.NoItem, string.Empty });
                }
            }
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }

    }
}
