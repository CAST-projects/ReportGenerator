using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("QUALITY_RULE_VIOLATIONS_BOOKMARKS")]
    public class QualityRuleViolationsBookmarks : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;
            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.

            string ruleId = options.GetOption("ID", "7788");
            const string bcId = "60017";
            int nbLimitTop = options.GetIntOption("COUNT", 5);

            bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
            RuleDescription rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, ruleId);
            string ruleName = rule.Name;

            rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);
            cellidx++;

            IEnumerable<Violation> results = reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.CurrentSnapshot.Href, ruleId, bcId, nbLimitTop, "$all");
            if (results != null)
            {
                var _violations = results as Violation[] ?? results.ToArray();
                if (_violations.Length != 0)
                {
                    int violation_counter = 0;
                    string domainId = reportData.CurrentSnapshot.DomainId;
                    string snapshotId = reportData.CurrentSnapshot.Id.ToString();
                    MetricsUtility.PopulateViolationsBookmarks(reportData, _violations, violation_counter, rowData, cellidx, ruleName, cellProps, hasPreviousSnapshot, domainId, snapshotId, ruleId);
                }
                else
                {
                    rowData.Add(Labels.NoItem);
                }
            }
            else
            {
                rowData.Add(Labels.NoItem);
            }

            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowData.Count,
                NbColumns = 1,
                Data = rowData,
                CellsAttributes = cellProps
            };

            return table;

        }
    }
}
