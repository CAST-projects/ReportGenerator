using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("IFPUG_FUNCTIONS")]
    class IfpugFunctions : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            TableDefinition resultTable = null;

            int nbLimitTop = -1;
            if (null == options || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = -1;
            }
            string type = null;
            if (null != options && options.ContainsKey("TYPE"))
            {
                type = options["TYPE"] ?? string.Empty;
                type = type.Trim();
                if (type.ToUpper() == "DF")
                {
                    type = "Data Function";
                }
                else if (type.ToUpper() == "TF")
                {
                    type = "Transaction";
                }
            }
            bool displayHeader = (options == null || !options.ContainsKey("HEADER") || "NO" != options["HEADER"]);

            IEnumerable<IfpugFunction> functions = reportData.SnapshotExplorer.GetIfpugFunctions(reportData.CurrentSnapshot.Href, string.IsNullOrEmpty(type) ? nbLimitTop : -1);

            List<string> rowData = new List<string>();

            if (displayHeader)
            {
                rowData.AddRange(new string[] { Labels.IFPUG_ElementType, Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.IFPUG_FPDetails, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });
            }

            int nbRows = 0;

            if (functions != null && functions.Any())
            {
                var exportedList = functions;
                if (!string.IsNullOrEmpty(type))
                {
                    exportedList = exportedList.Where(f => f.ElementType == type);
                }
                if (nbLimitTop > 0)
                {
                    exportedList = exportedList.Take(nbLimitTop);
                }
                foreach (var ifpugFunction in exportedList)
                {
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ElementType) ? " " : ifpugFunction.ElementType);
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
                rowData.AddRange(new string[] { Labels.NoItem, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            }

            resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = displayHeader,
                NbRows = nbRows + (displayHeader ? 1 : 0),
                NbColumns = 7,
                Data = rowData
            };

            return resultTable;
        }
    }
}
