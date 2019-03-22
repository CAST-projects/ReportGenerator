using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_CUSTOM_EXPRESSION")]
    public class PortfolioCustomExpression : TextBlock
    {
        #region METHODS

        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string _metricFormat = options.GetOption("FORMAT", "N2");
            string _params = options.GetOption("PARAMS", string.Empty);
            string _expr = options.GetOption("EXPR",string.Empty);
            string _aggregator = options.GetOption("AGGREGATOR", "AVERAGE");

            string[] lstParams = _params.Split(' ');

            if (string.IsNullOrEmpty(_params)) return Labels.NoData;
            if (reportData?.Applications == null) return Labels.NoData;
            Application[] _allApps = reportData.Applications;
            string[] strValues = new string[_allApps.Length];

            for (int k = 0; k < _allApps.Length; k++)
            {
                Application _app = _allApps[k];
                string _appExpr = _expr;
                Snapshot currentSnap = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                if (currentSnap == null) continue;
                for (int i = 0; i < lstParams.Length; i += 2)
                {
                    string param = lstParams[i + 1];
                    double? paramValue;

                    switch (lstParams[i])
                    {
                        case "SZ":
                            int sizingId = int.Parse(options.GetOption(lstParams[i + 1], "0"));
                            if (sizingId == 0)
                            {
                                paramValue = null;
                                break;
                            }
                            paramValue = MeasureUtility.GetSizingMeasure(currentSnap, sizingId);
                            break;
                            
                        case "QR":
                            int qrId = int.Parse(options.GetOption(lstParams[i + 1], "0"));
                            if (qrId == 0)
                            {
                                paramValue = null;
                                break;
                            }
                            paramValue = BusinessCriteriaUtility.GetMetricValue(currentSnap, qrId);
                            break;

                        case "BF":
                            string bfId = options.GetOption(lstParams[i + 1], string.Empty);
                            if (string.IsNullOrEmpty(bfId))
                            {
                                paramValue = null;
                                break;
                            }
                            var bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(currentSnap.Href, bfId).FirstOrDefault();
                            if (bfValue != null && bfValue.ApplicationResults.Any())
                            {
                                paramValue = bfValue.ApplicationResults[0].DetailResult.Value;
                            }
                            else
                            {
                                paramValue = null;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    if (paramValue != null)
                    {
                        _appExpr = _appExpr.Replace(param, paramValue.ToString());
                    }
                }
                DataTable dt = new DataTable();
                string value;
                try
                {
                    value = double.Parse(dt.Compute(_appExpr, "").ToString()).ToString(_metricFormat);
                }
                catch (EvaluateException)
                {
                    value = null;
                }
                
                if (!string.IsNullOrEmpty(value))
                {
                    strValues[k] = value;
                }
                else
                {
                    strValues[k] = Labels.NoData;
                }
            }


            if (_aggregator == "SUM")
            {
                double sumValues = strValues.Where(val => val != Labels.NoData).Sum(val => double.Parse(val));
                return sumValues.ToString(_metricFormat);
            }
            else
            {
                double sumValues = 0;
                int nbValues = 0;
                foreach (string val in strValues)
                {
                    if (val == Labels.NoData) continue;
                    nbValues++;
                    sumValues += double.Parse(val);
                }
                if (nbValues == 0) return Labels.NoData;
                var avgValues = sumValues / nbValues;
                return avgValues.ToString(_metricFormat);
            }
        }
        #endregion METHODS
    }
}
