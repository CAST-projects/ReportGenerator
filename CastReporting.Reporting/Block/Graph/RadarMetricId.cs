
/*
 *   Copyright (c) 2019 CAST
 *
 */

using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Graph
{
	[Block("RADAR_METRIC_ID")]
	public class RadarMetricId : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string[] qidList = options.GetOption("ID")?.Split('|') ;
            string _version = options.GetOption("SNAPSHOT", "BOTH");

            var rowData = new List<string> {null};

            if (reportData?.CurrentSnapshot == null) return null;

            if (_version == "CURRENT" || _version == "BOTH")
            {
                string currSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.CurrentSnapshot);
                rowData.Add(currSnapshotLabel);
            }

            if (reportData.PreviousSnapshot != null && (_version == "PREVIOUS" || _version == "BOTH"))
            {
                string prevSnapshotLabel = SnapshotUtility.GetSnapshotVersionNumber(reportData.PreviousSnapshot);
                rowData.Add(prevSnapshotLabel ?? Constants.No_Value);
            }

            int nbRow = 0;
            if (qidList != null)
            {
                foreach (string qid in qidList)
                {
                    int id = int.Parse(qid.Trim());
                    string qidName;
                    double? curRes;
                    double? prevRes;

                    switch (_version)
                    {
                        case "CURRENT":
                            qidName = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, id, true);
                            if (string.IsNullOrEmpty(qidName)) continue;
                            rowData.Add(qidName);
                            curRes = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot, id);
                            rowData.Add(curRes?.ToString() ?? Constants.Zero);
                            nbRow++;
                            break;
                        case "PREVIOUS":
                            if (reportData.PreviousSnapshot != null)
                            {
                                qidName = BusinessCriteriaUtility.GetMetricName(reportData.PreviousSnapshot, id, true);
                                if (string.IsNullOrEmpty(qidName)) continue;
                                rowData.Add(qidName);
                                prevRes = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, id);
                                rowData.Add(prevRes?.ToString() ?? Constants.Zero);
                                nbRow++;
                            }
                            break;
                        default:
                            qidName = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, id, true);
                            if (string.IsNullOrEmpty(qidName)) continue;
                            rowData.Add(qidName);
                            curRes = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot, id);
                            rowData.Add(curRes?.ToString() ?? Constants.Zero);
                            if (reportData.PreviousSnapshot != null)
                            {
                                prevRes = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, id);
                                rowData.Add(prevRes?.ToString() ?? Constants.Zero);
                            }
                            nbRow++;
                            break;
                    }
                }
            }

            int nbCol = _version == "CURRENT" || _version == "PREVIOUS" ? 2 : 3;
            if ((_version == "BOTH" || _version == "PREVIOUS") && reportData.PreviousSnapshot == null) nbCol--;

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = true,
                NbRows = nbRow + 1,
                NbColumns = nbCol,
                Data = rowData
            };

            return resultTable;
        }
     
    }

}
