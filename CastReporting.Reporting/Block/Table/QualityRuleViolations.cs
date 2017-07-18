using System.Collections.Generic;
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("QUALITY_RULE_VIOLATIONS")]
    public class QualityRuleViolations : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();

            string ruleId = options.GetOption("ID");
            string bcId = options.GetOption("BCID", "60013");
            int nbLimitTop = options.GetIntOption("COUNT", 10);
            bool shortName = options.GetOption("NAME","FULL") == "SHORT";
            bool previous = options.GetOption("SNAPSHOT", "CURRENT") == "PREVIOUS";
            bool hasPri = bcId.Equals("60013") || bcId.Equals("60014") || bcId.Equals("60016");

            bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
            string ruleName = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(ruleId), false);

            int nbCol = 1;
            rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);
            if (hasPri)
            {
                nbCol++;
                rowData.Add(Labels.PRI);
            }
            if (hasPreviousSnapshot && !previous)
            {
                nbCol++;
                rowData.Add(Labels.Status);
            }

            if (previous && !hasPreviousSnapshot)
            {
                rowData.Add(Constants.No_Data);
            }
            else
            {
                IEnumerable<Violation> results = hasPreviousSnapshot && previous ?
                    reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.PreviousSnapshot.Href, ruleId, bcId, nbLimitTop)
                    : reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.CurrentSnapshot.Href, ruleId, bcId, nbLimitTop);

                foreach (Violation _violation in results)
                {
                    rowData.Add(shortName ? _violation.Component.ShortName : _violation.Component.Name);
                    if (hasPri) rowData.Add(_violation.Component.PropagationRiskIndex.ToString("N0"));
                    if (hasPreviousSnapshot && !previous) rowData.Add(_violation.Diagnosis.Status);
                }
            }

            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbLimitTop + 1,
                NbColumns = nbCol,
                Data = rowData
            };

            return table;

        }
    }
}
