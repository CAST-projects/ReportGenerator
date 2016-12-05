using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("EFP")]
    internal class EFP : TableBlock
    {
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
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
                    case "ADDED":
                        type = "Added";
                        break;
                    case "DELETED":
                        type = "Deleted";
                        break;
                    case "MODIFIED":
                        type = "Modified";
                        break;
                    default:
                        type = string.Empty;
                        break;
                }
            }
            bool displayHeader = (options == null || !options.ContainsKey("HEADER") || "NO" != options["HEADER"]);

            IEnumerable<IfpugFunction> functions = reportData.SnapshotExplorer.GetIfpugFunctionsEvolutions(reportData.CurrentSnapshot.Href, string.IsNullOrEmpty(type) ? nbLimitTop : -1).ToList();

            List<string> rowData = new List<string>();

            if (displayHeader)
            {
                rowData.AddRange(new[] { Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });
            }

            int nbRows = 0;

            if (functions.Any())
            {
                var exportedList = functions;
                if (!string.IsNullOrEmpty(type))
                {
                    exportedList = exportedList.Where(f => f.ElementType.Contains(type));
                }
                if (nbLimitTop > 0)
                {
                    exportedList = exportedList.Take(nbLimitTop);
                }
                foreach (var ifpugFunctionevolution in exportedList)
                {
                    //rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.ElementType) ? " " : ifpugFunctionevolution.ElementType);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.ObjectName) ? " " : ifpugFunctionevolution.ObjectName);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.NoOfFPs) ? " " : ifpugFunctionevolution.NoOfFPs);
                    //rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.FPDetails) ? " " : ifpugFunctionevolution.FPDetails);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.ObjectType) ? " " : ifpugFunctionevolution.ObjectType);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.ModuleName) ? " " : ifpugFunctionevolution.ModuleName);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunctionevolution.Technology) ? " " : ifpugFunctionevolution.Technology);
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
                NbColumns = 5,
                Data = rowData
            };

            return resultTable;
        }
    }
}
