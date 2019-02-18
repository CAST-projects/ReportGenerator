using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
    [Block("AEFP_LIST")]
    public class AEFPList : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
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
                        type = "Transactional";
                        break;
                    default:
                        type = string.Empty;
                        break;
                }
            }
            string status = null;
            if (null != options && options.ContainsKey("STATUS"))
            {
                status = options["STATUS"] ?? string.Empty;
                status = status.Trim();
                switch (status.ToUpper())
                {
                    case "ADDED":
                        status = "Added";
                        break;
                    case "DELETED":
                        status = "Deleted";
                        break;
                    case "MODIFIED":
                        status = "Modified";
                        break;
                    default:
                        status = string.Empty;
                        break;
                }
            }

            // return all data because the filter cannot be applied now (no filter in url)
            IEnumerable<OmgFunction> functions = reportData.SnapshotExplorer.GetOmgFunctionsEvolutions(reportData.CurrentSnapshot.Href, -1)?.ToList();

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.IFPUG_ElementType, Labels.FunctionName, Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.ComplexityFactor, Labels.UpdatedArtifacts, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });

            int nbRows = 1;

            if (functions != null && functions.Any())
            {
                var exportedList = functions;
                if (!string.IsNullOrEmpty(type))
                {
                    exportedList = exportedList.Where(f => f.ElementType.Contains(type));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    exportedList = exportedList.Where(f => f.ElementType.Contains(status));
                }
                if (nbLimitTop > 0)
                {
                    exportedList = exportedList.Take(nbLimitTop);
                }
                foreach (OmgFunction omgFunction in exportedList)
                {
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ElementType) ? " " : omgFunction.ElementType);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.FunctionName) ? " " : omgFunction.FunctionName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectName) ? " " : omgFunction.ObjectName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.NoOfFPs) ? " " : omgFunction.NoOfFPs);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ComplexityFactor) ? " " : omgFunction.ComplexityFactor);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.UpdatedArtifacts) ? " " : omgFunction.UpdatedArtifacts);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectType) ? " " : omgFunction.ObjectType);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ModuleName) ? " " : omgFunction.ModuleName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.Technology) ? " " : omgFunction.Technology);
                    nbRows += 1;
                }
            }
            else
            {
                rowData.AddRange(new[] { Labels.NoItem, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = 9,
                Data = rowData
            };

            return resultTable;
        }
    }
}
