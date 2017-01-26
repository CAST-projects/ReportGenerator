using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.Domain;


namespace CastReporting.Reporting.Block.Table
{
    [Block("IFPUG_TRANSACTION_FUNCTIONS")]
    internal class IfpugTransactionFunctions : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = -1;
            }

            IEnumerable<IfpugFunction> functions = reportData.SnapshotExplorer.GetIfpugFunctions(reportData.CurrentSnapshot.Href, nbLimitTop)?.ToList();
            List<string> rowData = new List<string>();
            int nbRows = 0;

            if (functions != null && functions.Any())
            {
                IEnumerable<IfpugFunction> exportedList = (nbLimitTop <= 0) ? functions : functions.Take(nbLimitTop);
                foreach (var ifpugFunction in exportedList)
                {
                    if (ifpugFunction.ElementType == "Transaction")
                    {
                        rowData.Add(string.IsNullOrEmpty(ifpugFunction.ObjectName) ? " " : ifpugFunction.ObjectName);
                        rowData.Add(string.IsNullOrEmpty(ifpugFunction.NoOfFPs) ? " " : ifpugFunction.NoOfFPs);
                        rowData.Add(string.IsNullOrEmpty(ifpugFunction.FPDetails) ? " " : ifpugFunction.FPDetails);
                        rowData.Add(string.IsNullOrEmpty(ifpugFunction.ObjectType) ? " " : ifpugFunction.ObjectType);
                        rowData.Add(string.IsNullOrEmpty(ifpugFunction.ModuleName) ? " " : ifpugFunction.ModuleName);
                        rowData.Add(string.IsNullOrEmpty(ifpugFunction.Technology) ? " " : ifpugFunction.Technology);
                        nbRows += 1;
                    }
                }
            }
            else
            {
                rowData.AddRange(new[] { Labels.NoItem, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 6,
                Data = rowData
            };

            return resultTable;
        }
    }
}
