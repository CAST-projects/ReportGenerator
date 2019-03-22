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
    [Block("DELTA_COMPONENTS_LIST_BY_STATUS")]
    public class DeltaComponentsListByStatus : TableBlock
    {
        private int _nbRows;
        private int _nbLimit;

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();

            string status = options.GetOption("STATUS", "added").ToLower();
            _nbLimit = options.GetIntOption("COUNT", 10);

            string moduleName = options.GetOption("MODULE", string.Empty);
            string technoName = options.GetOption("TECHNOLOGY", string.Empty);
            if (moduleName != string.Empty && technoName != string.Empty)
            {
                moduleName = string.Empty;
                technoName = string.Empty;
            }

            string complexity = options.GetOption("COMPLEXITY", "all").ToLower();
            string currentSnapshotName = options.GetOption("CURRENT", string.Empty);
            string previousSnapshotName = options.GetOption("PREVIOUS", string.Empty);

            rowData.Add(Labels.ObjectName);
            rowData.Add(Labels.Complexity1);
            rowData.Add(Labels.SQLComplexity);
            rowData.Add(Labels.Granularity);
            rowData.Add(Labels.LackOfComments);
            rowData.Add(Labels.Coupling1);
            rowData.Add(Labels.NumberOfObjectUpdates);
            rowData.Add(Labels.ObjectFullName);
            _nbRows = 1;

            string currentSnapshotId = currentSnapshotName.Equals(string.Empty) ? reportData.CurrentSnapshot.GetId()
                : reportData.Application.Snapshots.FirstOrDefault(_ => _.Name.Equals(currentSnapshotName))?.GetId() ?? reportData.CurrentSnapshot.GetId();

            string previousSnapshotId = previousSnapshotName.Equals(string.Empty) ? reportData.PreviousSnapshot?.GetId()
                : reportData.Application.Snapshots.FirstOrDefault(_ => _.Name.Equals(previousSnapshotName))?.GetId() ?? reportData.PreviousSnapshot?.GetId();

            if (previousSnapshotId == null)
            {
                rowData.AddRange(new[]{Labels.NoPreviousSnapshot,string.Empty,string.Empty,string.Empty,string.Empty,string.Empty,string.Empty,string.Empty});
                return new TableDefinition {HasRowHeaders = false,HasColumnHeaders = true,NbRows = 2,NbColumns = 8,Data = rowData};
            }

            string[] allowedStatus = new[] { "added", "deleted", "updated"};
            if (!allowedStatus.Contains(status))
            {
                rowData.AddRange(new[] { Labels.StatusNotAllowed, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
                return new TableDefinition { HasRowHeaders = false, HasColumnHeaders = true, NbRows = 2, NbColumns = 8, Data = rowData };
            }

            if (!moduleName.Equals(string.Empty))
            {
                Module module = reportData.CurrentSnapshot.Modules.FirstOrDefault(_ => _.Name.Equals(moduleName));
                if (module != null)
                {
                    string moduleHref = module.DomainId + "/modules/" + module.Id;
                    rowData.AddRange(GetDeltaComponents(reportData, moduleHref, status, currentSnapshotId, previousSnapshotId, complexity));
                    return new TableDefinition { HasRowHeaders = false, HasColumnHeaders = true, NbRows = _nbRows, NbColumns = 8, Data = rowData };
                }
                else
                {
                    rowData.AddRange(new[] { Labels.ModuleNotFound, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
                    return new TableDefinition { HasRowHeaders = false, HasColumnHeaders = true, NbRows = 2, NbColumns = 8, Data = rowData };
                }
            }

            if (!technoName.Equals(string.Empty))
            {
                if (reportData.Application.Technologies.Contains(technoName))
                {
                    rowData.AddRange(GetDeltaComponents(reportData, reportData.Application.Href, status, currentSnapshotId, previousSnapshotId, complexity, technoName));
                    return new TableDefinition { HasRowHeaders = false, HasColumnHeaders = true, NbRows = _nbRows, NbColumns = 8, Data = rowData };
                }
                else
                {
                    rowData.AddRange(new[] { Labels.TechnoNotFound, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
                    return new TableDefinition { HasRowHeaders = false, HasColumnHeaders = true, NbRows = 2, NbColumns = 8, Data = rowData };
                }
            }

            rowData.AddRange(GetDeltaComponents(reportData, reportData.Application.Href, status, currentSnapshotId, previousSnapshotId, complexity));
            return new TableDefinition { HasRowHeaders = false, HasColumnHeaders = true, NbRows = _nbRows, NbColumns = 8, Data = rowData };
        }

        private IEnumerable<string> GetDeltaComponents(ReportData reportData, string href, string status, string currentSnapshotId, string previousSnapshotId, string complexity = "all", string technology = null)
        {
            List<string> dataList = new List<string>();
            IEnumerable<DeltaComponent> components = complexity.Equals("all") || !(new[] {"low", "moderate", "high", "very high"}.Contains(complexity))
                ? reportData.RuleExplorer.GetDeltaComponents(href, status, currentSnapshotId, previousSnapshotId, technology).OrderBy(_ => _.Name)
                : reportData.RuleExplorer.GetDeltaComponents(href, status, currentSnapshotId, previousSnapshotId, technology).Where(_ => _.Complexity.ToLower().Equals(complexity + " risk")).OrderBy(_ => _.Name);

            components = (_nbLimit != -1) ? components.Take(_nbLimit) : components;
            foreach (DeltaComponent component in components)
            {
                dataList.AddRange(new[]
                {
                    component.ShortName,
                    component.Complexity,
                    component.SqlComplexity,
                    component.Granularity,
                    component.LackOfComments,
                    component.Coupling,
                    component.NbOfUpdates.ToString(),
                    component.Name
                });
                _nbRows ++;
            }
            return dataList;
        }
    }
}
