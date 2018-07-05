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
    [Block("QUALITY_RULE_VIOLATIONS_BOOKMARKS")]
    public class QualityRuleViolationsBookmarks : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            string ruleId = options.GetOption("ID", "7788");
            string bcId = options.GetOption("BCID", "60013");
            int nbLimitTop = options.GetIntOption("COUNT", 5);
            bool shortName = options.GetOption("NAME","FULL") == "SHORT";
            bool previous = options.GetOption("SNAPSHOT", "CURRENT") == "PREVIOUS";
            int nbBookmarks = options.GetIntOption("NB_BOOKMARKS", 5);

            bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
            string ruleName = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(ruleId));

            rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);
            //cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.Blue });
            cellidx++;

            if (previous && !hasPreviousSnapshot)
            {
                rowData.Add(Constants.No_Data);
            }
            else
            {
                IEnumerable<Violation> results = hasPreviousSnapshot && previous ?
                    reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.PreviousSnapshot.Href, ruleId, bcId, nbLimitTop, "$all")
                    : reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.CurrentSnapshot.Href, ruleId, bcId, nbLimitTop, "$all");
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
                            rowData.Add(Labels.Violation + " #" + violation_counter);
                            cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.Gainsboro });
                            cellidx++;
                            rowData.Add(Labels.ObjectName + ": " + (shortName ? _violation.Component.ShortName : _violation.Component.Name));
                            cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.White });
                            cellidx++;
                            if (hasPreviousSnapshot && !previous)
                            {
                                rowData.Add(Labels.Status + ": " + _violation.Diagnosis.Status);
                                cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.White });
                                cellidx++;
                            }
                            // Add lines containing the file path and source code around the violation
                            IEnumerable<IEnumerable<CodeBookmark>> bookmarks = reportData.SnapshotExplorer.GetBookmarks(domainId,
                                _violation.Component.GetComponentId(), snapshotId, ruleId);

                            if (bookmarks != null)
                            {
                                //int primary_counter = 0;
                                IEnumerable<CodeBookmark>[] _codeBookmarkses = bookmarks as IEnumerable<CodeBookmark>[] ?? bookmarks.ToArray();
                                
                                if (_codeBookmarkses.Any())
                                {
                                    /*
                                    primary_counter++;
                                    rowData.Add(Labels.Defect + " #" + primary_counter);
                                    cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.SkyBlue });
                                    cellidx++;
                                    */
                                    int defects_counter = 0;
                                    foreach (IEnumerable<CodeBookmark> _codeBookmarks in _codeBookmarkses)
                                    {
                                        defects_counter++;
                                        if (defects_counter != -1 && defects_counter > nbBookmarks) continue;

                                        IEnumerable<CodeBookmark> _bookmarks = _codeBookmarks.ToList();
                                        int secondary_counter = 0;
                                        // int nb_bk = _bookmarks.Count();
                                        foreach (CodeBookmark _bookmark in _bookmarks)
                                        {
                                            secondary_counter++;
                                            if (nbBookmarks != -1 && secondary_counter > nbBookmarks) continue;
                                            /*
                                            if (secondary_counter > 1)
                                            {
                                                rowData.Add(Labels.AdditionalInformation + " #" + secondary_counter + "/" + nb_bk);
                                                cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.Cyan });
                                                cellidx++;
                                            }
                                            */
                                            rowData.Add(Labels.FilePath + ": " + _bookmark.CodeFragment.CodeFile.Name);
                                            cellProps.Add(new CellAttributes() { Index = cellidx, BackgroundColor = Color.Gainsboro });
                                            cellidx++;
                                            Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookmark);

                                            foreach (KeyValuePair<int, string> codeLine in codeLines)
                                            {
                                                rowData.Add(codeLine.Key + " : " + codeLine.Value);
                                                cellProps.Add(codeLine.Key == _bookmark.CodeFragment.StartLine
                                                    ? new CellAttributes() {Index = cellidx, BackgroundColor = Color.LightYellow}
                                                    : new CellAttributes() {Index = cellidx, BackgroundColor = Color.White});
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
                    else
                    {
                        rowData.Add(Labels.NoItem);
                    }
                }
                else
                {
                    rowData.Add(Labels.NoItem);
                }
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
