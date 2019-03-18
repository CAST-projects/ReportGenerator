using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("IFPUG_FUNCTIONS")]
    public class IfpugFunctions : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = -1;
            }
            string type = null;
            if (null != options && options.ContainsKey("TYPE"))
            {
                type = options["TYPE"] ?? string.Empty;
                type = type.Trim();
                switch (type.ToUpper())
                {
                    case "DF":
                        type = "Data Function";
                        break;
                    case "TF":
                        type = "Transaction";
                        break;
                    default:
                        type = string.Empty;
                        break;
                }
            }
            bool displayHeader = (options == null || !options.ContainsKey("HEADER") || "NO" != options["HEADER"]);

            IEnumerable<IfpugFunction> functions = reportData.SnapshotExplorer.GetIfpugFunctions(reportData.CurrentSnapshot.Href, string.IsNullOrEmpty(type) ? nbLimitTop : -1)?.ToList();

            List<string> rowData = new List<string>();

            if (displayHeader)
            {
                rowData.AddRange(new[] { Labels.IFPUG_ElementType, Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.IFPUG_FPDetails, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });
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
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.NbOfFPs) ? 
                        string.IsNullOrEmpty(ifpugFunction.NoOfFPs) ? 
                        string.IsNullOrEmpty(ifpugFunction.Afps) ? " " : ifpugFunction.Afps
                        : ifpugFunction.NoOfFPs 
                        : ifpugFunction.NbOfFPs);
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
                HasColumnHeaders = displayHeader,
                NbRows = nbRows + (displayHeader ? 1 : 0),
                NbColumns = 7,
                Data = rowData
            };

            return resultTable;
        }
    }
}
