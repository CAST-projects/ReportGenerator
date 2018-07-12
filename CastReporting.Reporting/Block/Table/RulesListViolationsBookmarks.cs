using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Languages;

namespace CastReporting.Reporting.Block.Table
{
    [Block("LIST_RULES_VIOLATIONS_BOOKMARKS")]
    public class RulesListViolationsBookmarks : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;
            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.

            List<string> metrics = options.GetOption("METRICS").Trim().Split('|').ToList();
            List<string> qualityRules = MetricsUtility.BuildRulesList(reportData, metrics);

            rowData.Add(Labels.Violations);
            cellidx++;

            if (qualityRules.Count > 0)
            {
                const string bcId = "60017";
                int nbLimitTop = options.GetIntOption("COUNT", 5);
                bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;

                foreach (string _metric in qualityRules)
                {
                    RuleDescription rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, _metric);
                    string ruleName = rule.Name;
                    if (ruleName == null) continue;
                    int metricId;
                    if (!int.TryParse(_metric, out metricId)) continue;
                    ViolStatMetricIdDTO violStats = RulesViolationUtility.GetViolStat(reportData.CurrentSnapshot, metricId);
                    if (violStats == null) continue;

                    rowData.Add("");
                    cellidx++;
                    rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);
                    cellProps.Add(new CellAttributes(cellidx, Color.Gray, Color.White, "bold"));
                    cellidx++;
                    rowData.Add(Labels.ViolationsCount + ": " + violStats.TotalViolations);
                    cellProps.Add(new CellAttributes(cellidx, Color.White));
                    cellidx++;
                    if (!string.IsNullOrWhiteSpace(rule.Rationale))
                    {
                        rowData.Add(Labels.Rationale + ": ");
                        cellProps.Add(new CellAttributes(cellidx, Color.LightGray));
                        cellidx++;
                        rowData.Add(rule.Rationale);
                        cellProps.Add(new CellAttributes(cellidx, Color.White));
                        cellidx++;
                    }
                    rowData.Add(Labels.Description + ": ");
                    cellProps.Add(new CellAttributes(cellidx, Color.LightGray));
                    cellidx++;
                    rowData.Add(rule.Description);
                    cellProps.Add(new CellAttributes(cellidx, Color.White));
                    cellidx++;
                    if (!string.IsNullOrWhiteSpace(rule.Remediation))
                    {
                        rowData.Add(Labels.Remediation + ": ");
                        cellProps.Add(new CellAttributes(cellidx, Color.LightGray));
                        cellidx++;
                        rowData.Add(rule.Remediation);
                        cellProps.Add(new CellAttributes(cellidx, Color.White));
                        cellidx++;
                    }

                    IEnumerable<Violation> results = reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.CurrentSnapshot.Href, _metric, bcId, nbLimitTop, "$all");
                    if (results != null)
                    {
                        var _violations = results as Violation[] ?? results.ToArray();
                        if (_violations.Length != 0)
                        {
                            int violation_counter = 0;
                            string domainId = reportData.CurrentSnapshot.DomainId;
                            string snapshotId = reportData.CurrentSnapshot.Id.ToString();
                            foreach (Violation _violation in _violations)
                            {
                                violation_counter++;
                                rowData.Add("");
                                cellidx++;
                                rowData.Add(Labels.Violation + " #" + violation_counter + "    " + ruleName);
                                cellProps.Add(new CellAttributes(cellidx, Color.Gainsboro));
                                cellidx++;
                                rowData.Add(Labels.ObjectName + ": " + _violation.Component.Name);
                                cellProps.Add(new CellAttributes(cellidx, Color.White));
                                cellidx++;

                                TypedComponent objectComponent = reportData.SnapshotExplorer.GetTypedComponent(reportData.CurrentSnapshot.DomainId, _violation.Component.GetComponentId(), reportData.CurrentSnapshot.GetId());
                                rowData.Add(Labels.IFPUG_ObjectType + ": " + objectComponent.Type.Label);
                                cellProps.Add(new CellAttributes(cellidx, Color.White));
                                cellidx++;

                                if (hasPreviousSnapshot)
                                {
                                    rowData.Add(Labels.Status + ": " + _violation.Diagnosis.Status);
                                    cellProps.Add(new CellAttributes(cellidx, Color.White));
                                    cellidx++;
                                }
                                // Add lines containing the file path and source code around the violation
                                IEnumerable<IEnumerable<CodeBookmark>> bookmarks = reportData.SnapshotExplorer.GetBookmarks(domainId,
                                    _violation.Component.GetComponentId(), snapshotId, _metric);

                                if (bookmarks != null)
                                {
                                    IEnumerable<CodeBookmark>[] _codeBookmarkses = bookmarks as IEnumerable<CodeBookmark>[] ?? bookmarks.ToArray();

                                    if (_codeBookmarkses.Any())
                                    {
                                        foreach (IEnumerable<CodeBookmark> _codeBookmarks in _codeBookmarkses)
                                        {
                                            IEnumerable<CodeBookmark> _bookmarks = _codeBookmarks.ToList();
                                            foreach (CodeBookmark _bookmark in _bookmarks)
                                            {
                                                rowData.Add(Labels.FilePath + ": " + _bookmark.CodeFragment.CodeFile.Name);
                                                cellProps.Add(new CellAttributes(cellidx, Color.Lavender));
                                                cellidx++;
                                                Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookmark);

                                                foreach (KeyValuePair<int, string> codeLine in codeLines)
                                                {
                                                    rowData.Add(codeLine.Key + " : " + codeLine.Value);
                                                    cellProps.Add(codeLine.Key == _bookmark.CodeFragment.StartLine
                                                        ? new CellAttributes(cellidx, Color.LightYellow)
                                                        : new CellAttributes(cellidx, Color.White));
                                                    cellidx++;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        rowData.Add(Labels.NoBookmark);
                                        cellidx++;
                                    }
                                }
                                else
                                {
                                    rowData.Add(Labels.NoBookmark);
                                    cellidx++;
                                }
                            }
                        }
                    }
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
