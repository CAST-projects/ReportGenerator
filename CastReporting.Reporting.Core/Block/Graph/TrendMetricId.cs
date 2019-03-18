
/*
 *   Copyright (c) 2019 CAST
 *
 */

using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CastReporting.BLL.Computing;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Graph
{
   
    [Block("TREND_METRIC_ID")]
    public class TrendMetricId : GraphBlock
    {
     
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int count = 0;         

            string[] qidList = options.GetOption("QID")?.Split('|');
            string[] sidList = options.GetOption("SID")?.Split('|');
            string[] bidList = options.GetOption("BID")?.Split('|');

            // we can add the header only after getting the data, because names are in the data
            var rowData = new List<string>();

            #region get Metric names

            Dictionary<string,string> names = new Dictionary<string, string>();

            if (qidList != null)
            {
                foreach (string id in qidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    if (string.IsNullOrEmpty(BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(id)))) continue;
                    names[id] = BusinessCriteriaUtility.GetMetricName(reportData.CurrentSnapshot, int.Parse(id));
                }
            }
            if (sidList != null)
            {
                foreach (string id in sidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    if (string.IsNullOrEmpty(MeasureUtility.GetSizingMeasureName(reportData.CurrentSnapshot, int.Parse(id)))) continue;
                    names[id] = MeasureUtility.GetSizingMeasureName(reportData.CurrentSnapshot, int.Parse(id));
                }
            }

            // No background facts for technologies
            if (bidList != null)
            {
                foreach (string id in bidList)
                {
                    if (names.Keys.Contains(id)) continue;
                    Result bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.CurrentSnapshot.Href, id, true, true).FirstOrDefault();
                    if (bfResult == null || !bfResult.ApplicationResults.Any()) continue;
                    if (string.IsNullOrEmpty(bfResult.ApplicationResults[0].Reference.Name)) continue;
                    names[id] = bfResult.ApplicationResults[0].Reference.Name;
                }
            }

            rowData.Add(" ");
            rowData.AddRange(names.Values);

            #endregion

            int nbSnapshots = reportData?.Application.Snapshots?.Count() ?? 0;
            if (nbSnapshots > 0)
            {

                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once AssignNullToNotNullAttribute
                foreach (Snapshot snapshot in reportData.Application.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                {
                    rowData.Add(snapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    // iterate in QID
                    if (qidList != null)
                    {
                        foreach (string id in qidList)
                        {
                            if (!names.Keys.Contains(id)) continue;
                            ApplicationResult res = reportData.SnapshotExplorer.GetQualityIndicatorResults(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                            string idValue = res?.DetailResult?.Grade?.ToString("N2") ?? Constants.Zero;
                            if (!values.Keys.Contains(id))
                                values.Add(id, idValue);
                        }
                    }

                    // iterate in SID
                    if (sidList != null)
                    {
                        foreach (string id in sidList)
                        {
                            if (!names.Keys.Contains(id)) continue;
                            ApplicationResult res = reportData.SnapshotExplorer.GetSizingMeasureResults(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                            string idValue = res?.DetailResult?.Value?.ToString("F0") ?? Constants.Zero;
                            if (!values.Keys.Contains(id))
                                values.Add(id, idValue);
                        }
                    }

                    // iterate in BID
                    if (bidList != null)
                    {
                        foreach (string id in bidList)
                        {
                            if (!names.Keys.Contains(id)) continue;
                            ApplicationResult res = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                            // F0 as format to avoid the ',' that make graph build crash
                            string idValue = res?.DetailResult?.Value?.ToString("F0") ?? Constants.Zero;
                            if (!values.Keys.Contains(id))
                                values.Add(id, idValue);
                        }
                    }

                    rowData.AddRange(values.Values);

                }
                count = nbSnapshots;
            }

            #region just 1 snapshot
            // if there is only one snapshot, a fake snapshot is added with same data to have a line and not a point in the graph
			if (count == 1)
            {
                string[] range = new string[names.Count + 1];

                for (int k = 0; k < names.Count + 1; k++)
                {
                    range[k] = rowData[k + names.Count + 1];
                }

                rowData.AddRange(range);
                count = count +1;
            }
            #endregion just 1 snapshot
            
            TableDefinition resultTable = new TableDefinition {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows =count + 1 ,
                NbColumns = names.Count + 1,
                Data = rowData
            };


            return resultTable;
        }
        #endregion METHODS

    }
}
