using System.Collections.Generic;
using System.Linq;
using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("REMOVED_VIOLATIONS_LIST")]
    public class RemovedViolationsListByBC : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();

            string bcId = options.GetOption("BCID", "60017"); // by default, TQI
            int nbLimitTop = options.GetIntOption("COUNT", 50); // -1 for all removed violations
            int nbRows;
            string criticity = options.GetOption("CRITICITY", "all"); // should be "c" for critical, "nc" for non critical, and any other thing for both

            rowData.Add(Labels.ViolationStatus);
            rowData.Add(Labels.ExclusionStatus);
            rowData.Add(Labels.ActionStatus);
            rowData.Add(Labels.RuleName);
            rowData.Add(Labels.Weight);
            rowData.Add(Labels.ObjectName);
            rowData.Add(Labels.ObjectStatus);

            if (!VersionUtil.Is18Compatible(reportData.ServerVersion))
            {
                LogHelper.Instance.LogError("Bad version of RestAPI. Should be 1.8 at least for component REMOVED_VIOLATIONS_LIST");
                rowData.Add(Labels.NoData);
                for (int i = 0; i < 6; i++)
                {
                    rowData.Add(string.Empty);
                }
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 7,
                    Data = rowData
                };
            }

            List<Violation> removedViolations = reportData.SnapshotExplorer.GetRemovedViolationsbyBC(reportData.CurrentSnapshot.Href, bcId, nbLimitTop,criticity).ToList();

            List<RuleDetails> rulesDetails = reportData.RuleExplorer.GetRulesDetails(reportData.CurrentSnapshot.DomainId, int.Parse(bcId), reportData.CurrentSnapshot.Id).ToList();

            if (removedViolations.Count != 0)
            {
                foreach (Violation _violation in removedViolations)
                {
                    string[] fragments = _violation.RulePattern.Href.Split('/');
                    int key = int.Parse(fragments[fragments.Length - 1]);
                    string weight = rulesDetails.FirstOrDefault(_ => _.Key == key)?.CompoundedWeight.ToString();

                    rowData.Add(_violation.Diagnosis?.Status ?? Constants.No_Value);
                    rowData.Add(_violation.ExclusionRequest?.Status ?? Constants.No_Value);
                    rowData.Add(_violation.RemedialAction?.Status ?? Constants.No_Value);
                    rowData.Add(_violation.RulePattern?.Name ?? Constants.No_Value);
                    rowData.Add(weight);
                    rowData.Add(_violation.Component?.Name ?? Constants.No_Value);
                    rowData.Add(_violation.Component?.Status ?? Constants.No_Value);
                }
                nbRows = removedViolations.Count + 1;
            }
            else
            {
                rowData.Add(Labels.NoItem);
                for (int i = 1; i < 8; i++)
                {
                    rowData.Add(string.Empty);
                }
                nbRows = 2;
            }

            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = 7,
                Data = rowData
            };

            return table;

        }
    }
}
