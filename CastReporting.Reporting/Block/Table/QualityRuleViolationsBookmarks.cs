using System.Collections.Generic;
using System.Linq;
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

            string ruleId = options.GetOption("ID", "7788");
            string bcId = options.GetOption("BCID", "60013");
            int nbLimitTop = options.GetIntOption("COUNT", 5);
            bool shortName = options.GetOption("NAME","FULL") == "SHORT";
            bool previous = options.GetOption("SNAPSHOT", "CURRENT") == "PREVIOUS";
            bool hasPri = bcId.Equals("60013") || bcId.Equals("60014") || bcId.Equals("60016");
            int nbBookmarks = options.GetIntOption("NB_BOOKMARKS", 5);

            bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
            string ruleName = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(ruleId));

            rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);

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
                            rowData.Add(shortName ? _violation.Component.ShortName : _violation.Component.Name);
                            if (hasPri) rowData.Add(Labels.PRI + ":" +_violation.Component.PropagationRiskIndex.ToString("N0"));
                            if (hasPreviousSnapshot && !previous) rowData.Add(Labels.Status + ":" + _violation.Diagnosis.Status);
                            // Add lines containing the file path and source code around the violation
                            IEnumerable<IEnumerable<CodeBookmark>> bookmarks = reportData.SnapshotExplorer.GetBookmarks(domainId,
                                _violation.Component.GetComponentId(), snapshotId, ruleId);

                            if (bookmarks != null)
                            {
                                int primary_counter = 0;
                                IEnumerable<CodeBookmark>[] _codeBookmarkses = bookmarks as IEnumerable<CodeBookmark>[] ?? bookmarks.ToArray();
                                
                                if (_codeBookmarkses.Any())
                                {
                                    primary_counter++;
                                    rowData.Add(Labels.Defect + " #" + primary_counter);
                                    foreach (IEnumerable<CodeBookmark> _codeBookmarks in _codeBookmarkses)
                                    {
                                        IEnumerable<CodeBookmark> _bookmarks = _codeBookmarks.ToList();
                                        int secondary_counter = 0;
                                        int nb_bk = _bookmarks.Count();
                                        foreach (CodeBookmark _bookmark in _bookmarks)
                                        {
                                            secondary_counter++;
                                            if (secondary_counter > nbBookmarks) continue;
                                            if (secondary_counter > 1) rowData.Add(Labels.AdditionalInformation + " #" + secondary_counter + "/" + nb_bk);
                                            rowData.Add(Labels.FilePath + " : " + _bookmark.CodeFragment.CodeFile.Name);
                                            // TODO :
                                            // GetSourceCodeBookmark doit retourner un tableau de int, string contenant le numéro de la ligne et la ligne (sinon pas de passage à la ligne, ni numéro et c'est tout moche
                                            // ensuite on fait un addRange dessus dans le rowData
                                            Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookmark);
                                            rowData.AddRange(codeLines.Select(codeLine => codeLine.Key.ToString() + " : " + codeLine.Value));
                                        }
                                    }
                                }
                                else
                                {
                                    rowData.Add(Labels.NoBookmark);
                                }
                            }
                            else
                            {
                                rowData.Add(Labels.NoBookmark);
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
                NbRows = nbLimitTop + 1,
                NbColumns = 1,
                Data = rowData
            };

            return table;

        }
    }
}
