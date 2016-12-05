using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("IFPUG_DATA_FUNCTIONS")]
    internal class IfpugDataFunctions : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = -1;
            }

            IEnumerable<IfpugFunction> functions = reportData.SnapshotExplorer.GetIfpugFunctions(reportData.CurrentSnapshot.Href, nbLimitTop).ToList();
            List<string> rowData = new List<string>();
            //List<string> rowData = new List<string>(new string[] { Labels.IFPUG_ElementType, Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.IFPUG_FPDetails, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });
            int nbRows = 0;

            if (functions.Any())
            {
                IEnumerable<IfpugFunction> exportedList = (nbLimitTop <= 0) ? functions : functions.Take(nbLimitTop);
                foreach (var ifpugFunction in exportedList)
                {
                    if (ifpugFunction.ElementType != "Data Function") continue;
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ObjectName) ? " " : ifpugFunction.ObjectName);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.NoOfFPs) ? " " : ifpugFunction.NoOfFPs);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.FPDetails) ? " " : ifpugFunction.FPDetails);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ObjectType) ? " " : ifpugFunction.ObjectType);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ModuleName) ? " " : ifpugFunction.ModuleName);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.Technology) ? " " : ifpugFunction.Technology);
                    nbRows += 1;
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
