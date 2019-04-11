using System.Collections.Generic;
using System.Linq;
using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Table
{
    [Block("AETP_LIST")]
    public class AETPList : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
            }

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.ObjectName, Labels.ObjectFullName, Labels.IFPUG_ObjectType, Labels.Status, Labels.EffortComplexity, Labels.EquivalenceRatio, Labels.AEP });
            int nbRows = 1;

            if (!VersionUtil.Is19Compatible(reportData.ServerVersion))
            {
                LogHelper.Instance.LogError("Bad version of RestAPI. Should be 1.9 at least for component AETP_LIST");
                rowData.Add(Labels.NoData);
                for (int i = 0; i < 6; i++)
                {
                    rowData.Add(string.Empty);
                }
                nbRows++;
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbRows,
                    NbColumns = 7,
                    Data = rowData
                };
            }

            IEnumerable<OmgFunctionTechnical> functions = reportData.SnapshotExplorer.GetOmgFunctionsTechnical(reportData.CurrentSnapshot.Href, nbLimitTop)?.ToList();
            if (functions?.Count() > 0)
            { 
                foreach (OmgFunctionTechnical omgFunction in functions)
                {
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectName) ? Constants.No_Data : omgFunction.ObjectName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectFullName) ? Constants.No_Data : omgFunction.ObjectFullName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectType) ? Constants.No_Data : omgFunction.ObjectType);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectStatus) ? Constants.No_Data : omgFunction.ObjectStatus);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.EffortComplexity) ? Constants.No_Data : omgFunction.EffortComplexity.FormatStringDoubleIntoString());
                    rowData.Add(string.IsNullOrEmpty(omgFunction.EquivalenceRatio) ? Constants.No_Data : omgFunction.EquivalenceRatio.FormatStringDoubleIntoString());
                    rowData.Add(string.IsNullOrEmpty(omgFunction.AepCount) ? Constants.No_Data : omgFunction.AepCount?.FormatStringDoubleIntoString());
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
                NbColumns = 7,
                Data = rowData
            };

            return resultTable;
        }


    }
}
