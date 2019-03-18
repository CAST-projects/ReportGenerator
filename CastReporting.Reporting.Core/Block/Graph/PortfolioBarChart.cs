using System;
using System.Collections.Generic;
using System.Linq;
using Cast.Util.Log;
using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("PF_BAR_CHART")]
    public class PortfolioBarChart : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string metricId = options.GetOption("METRIC");
            bool firstLoop = true;
            var rowData = new List<string>();
            int cntRow = 1;
            

            Application[] _allApps = reportData.Applications;
            foreach (Application _app in _allApps)
            {
                try
                {
                    Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                    if (_snapshot == null) continue;
                    SimpleResult res = MetricsUtility.GetMetricNameAndResult(reportData, _snapshot, metricId,null,null,false);
                    if (res == null) continue;
                    if (firstLoop)
                    {
                        rowData.Add(" ");
                        rowData.Add(res.name);
                        firstLoop = false;
                    }
                    cntRow++;
                    rowData.Add(_app.Name);
                    rowData.Add(res.result?.ToString());
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogInfo(ex.Message);
                    LogHelper.Instance.LogInfo(Labels.NoSnapshot);
                }
            }


            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = cntRow,
                NbColumns = 2,
                Data = rowData,
                GraphOptions = null
            };
            return resultTable;
        }
    }
}
