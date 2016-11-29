
/*
 *   Copyright (c) 2016 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Graph
{
   
    [Block("TREND_METRIC_ID")]
    class TrendMetricId : GraphBlock
    {
     
        #region METHODS
        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int count = 0;         

            string[] qidList = options.GetOption("QID")?.Split('|');
            string[] sidList = options.GetOption("SID")?.Split('|');
            string[] bidList = options.GetOption("BID")?.Split('|');

            // we can add the header only after getting the data, because names are in the data
            var rowData = new List<string>();

            Dictionary<string,string> names = new Dictionary<string, string>();
            bool getIdNames = true;

			int nbSnapshots = reportData?.Application.Snapshots?.Count() ?? 0;
            if (nbSnapshots > 0)
            {

                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once AssignNullToNotNullAttribute
                foreach (Snapshot snapshot in reportData.Application.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot))
                {
                    string snapshotDate = snapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty;
                    // names at first iteration
                    if (getIdNames)
                    {
                        // iterate in QID
                        if (qidList != null)
                        {
                            foreach (string id in qidList)
                            {
                                ApplicationResult res = reportData.SnapshotExplorer.GetQualityIndicatorResults(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                                if (res == null) continue;
                                string idName = res.Reference.ShortName ?? res.Reference.Name;
                                if (!names.Keys.Contains(id))
                                    names.Add(id, idName);
                            }
                        }

                        // iterate in SID
                        if (sidList != null)
                        {
                            foreach (string id in sidList)
                            {
                                ApplicationResult res = reportData.SnapshotExplorer.GetSizingMeasureResults(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                                if (res == null) continue;
                                string idName = res.Reference.ShortName ?? res.Reference.Name;
                                if (!names.Keys.Contains(id))
                                    names.Add(id, idName);
                            }
                        }

                        // iterate in BID
                        if (bidList != null)
                        {
                            foreach (string id in bidList)
                            {
                                ApplicationResult res = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                                if (res == null) continue;
                                string idName = res.Reference.ShortName ?? res.Reference.Name;
                                if (!names.Keys.Contains(id))
                                    names.Add(id, idName);
                            }
                        }

                        // add names in rowData
                        string[] headers = new string[names.Count + 1];
                        headers[0] = " ";
                        int j = 1;
                        foreach (string key in names.Keys)
                        {
                            headers[j] = names[key];
                            j++;
                        }
                        rowData.AddRange(headers);
                        getIdNames = false;

                    }
                    // values

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    // iterate in QID
                    if (qidList != null)
                    {
                        foreach (string id in qidList)
                        {
                            ApplicationResult res = reportData.SnapshotExplorer.GetQualityIndicatorResults(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                            if (res == null) continue;
                            string idValue = res.DetailResult?.Grade.ToString("N2") ?? Constants.No_Value;
                            if (!values.Keys.Contains(id))
                                values.Add(id, idValue);
                        }
                    }

                    // iterate in SID
                    if (sidList != null)
                    {
                        foreach (string id in sidList)
                        {
                            ApplicationResult res = reportData.SnapshotExplorer.GetSizingMeasureResults(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                            if (res == null) continue;
                            string idValue = res.DetailResult?.Value.ToString("F0") ?? Constants.No_Value;
                            if (!values.Keys.Contains(id))
                                values.Add(id, idValue);
                        }
                    }

                    // iterate in BID
                    if (bidList != null)
                    {
                        foreach (string id in bidList)
                        {
                            ApplicationResult res = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, id.Trim())?.FirstOrDefault()?.ApplicationResults?.FirstOrDefault();
                            if (res == null) continue;
                            // F0 as format to avoid the ',' that make graph build crash
                            string idValue = res.DetailResult?.Value.ToString("F0") ?? Constants.No_Value;
                            if (!values.Keys.Contains(id))
                                values.Add(id, idValue);
                        }
                    }

                    // ajouter les nom de res dans le rowdata
                    string[] idvalues = new string[names.Count + 1];
                    idvalues[0] = snapshotDate;
                    int k = 1;
                    foreach (string key in values.Keys)
                    {
                        idvalues[k] = values[key];
                        k++;
                    }
                    rowData.AddRange(idvalues);

                }
                count = nbSnapshots;
            }

            #region just 1 snapshot
            // if there is only one snapshot, a fake snapshot is added with same data to have a line and not a point in the graph
			if (count == 1)
            {
                string[] range = new string[rowData.Count];
                int k = 0;

                foreach (string row in rowData)
                {
                    range[k] = row;
                    k++;
                }

                // ReSharper disable once PossibleNullReferenceException
                string prevSnapshotDate = reportData.CurrentSnapshot.Annotation.Date.DateSnapShot?.ToOADate().ToString(CultureInfo.CurrentCulture) ?? string.Empty;
                range[0] = prevSnapshotDate;

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
