﻿using System.Collections.Generic;
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
    [Block("VIOLATIONS_LIST")]
    public class ViolationsListByBC : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();

            string[] bcIds = options.GetOption("BCID") != null? options.GetOption("BCID").Trim().Split('|') : new[] { "60016" }; // by default, security
            int nbLimitTop = options.GetOption("COUNT") == "ALL" ? -1 : options.GetIntOption("COUNT", 10);
            bool shortName = options.GetOption("NAME","FULL").Equals("SHORT");
            bool hasPri = bcIds.Contains("60013") || bcIds.Contains("60014") || bcIds.Contains("60016");
            string filter = options.GetOption("FILTER", "ALL");
            bool critical = options.GetOption("VIOLATIONS", "CRITICAL").Equals("CRITICAL");
            string module = options.GetOption("MODULE");
            string[] technos = options.GetOption("TECHNOLOGIES") != null ? options.GetOption("TECHNOLOGIES").Trim().Split('|') : new[] { "$all" };

            rowData.Add(Labels.ViolationStatus);
            if (hasPri) rowData.Add(Labels.PRI);
            rowData.Add(Labels.ExclusionStatus);
            rowData.Add(Labels.ActionStatus);
            rowData.Add(Labels.RuleName);
            rowData.Add(Labels.BusinessCriterionName);
            rowData.Add(Labels.ObjectName);
            rowData.Add(Labels.ObjectStatus);

            List<Violation> results = new List<Violation>();

            
            foreach (string _bcid in bcIds)
            {
                Module mod = (module != null) ? reportData.CurrentSnapshot.Modules.FirstOrDefault(m => m.Name.Equals(module)) : null;
                string href = (mod == null) ? reportData.CurrentSnapshot.Href : mod.Href;

                string technologies = technos.Aggregate(string.Empty, (current, techno) => (current.Equals(string.Empty)) ? techno : current + "," + techno);

                IEnumerable <Violation> bcresults = critical ? 
                    reportData.SnapshotExplorer.GetViolationsListIDbyBC(href, "(critical-rules)", _bcid, -1, "(" + technologies + ")")
                    : reportData.SnapshotExplorer.GetViolationsListIDbyBC(href, "(nc:" + _bcid + ",cc:" + _bcid + ")", _bcid, -1,"(" + technologies + ")") ;

                switch (filter)
                {
                    case "ADDED":
                        bcresults = bcresults.Where(_ => _.Diagnosis.Status.Equals("added"));
                        break;
                    case "UNCHANGED":
                        bcresults = bcresults.Where(_ => _.Diagnosis.Status.Equals("unchanged"));
                        break;
                    case "UPDATED":
                        bcresults = bcresults.Where(_ => _.Diagnosis.Status.Equals("updated"));
                        break;
                }
                var _violations = bcresults.ToList();
                foreach (Violation _bcresult in _violations)
                {
                    _bcresult.Component.PriBusinessCriterion = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(_bcid));
                }
                results.AddRange(_violations);
            }

            results = nbLimitTop != -1 ? results.Take(nbLimitTop).ToList() : results;

            if (results.Count != 0)
            {
                foreach (Violation _violation in results)
                {
                    rowData.Add(_violation.Diagnosis?.Status ?? Constants.No_Value);
                    if (hasPri) rowData.Add(_violation.Component?.PropagationRiskIndex.ToString("N0"));
                    rowData.Add(_violation.ExclusionRequest?.Status ?? Constants.No_Value);
                    rowData.Add(_violation.RemedialAction?.Status ?? Constants.No_Value);
                    rowData.Add(_violation.RulePattern?.Name ?? Constants.No_Value);
                    rowData.Add(_violation.Component?.PriBusinessCriterion ?? Constants.No_Value);
                    rowData.Add(shortName ? _violation.Component?.ShortName : _violation.Component?.Name ?? Constants.No_Value);
                    rowData.Add(_violation.Component?.Status ?? Constants.No_Value);
                }
            }

            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = results.Count + 1,
                NbColumns = hasPri ? 8 : 7,
                Data = rowData
            };

            return table;

        }
    }
}