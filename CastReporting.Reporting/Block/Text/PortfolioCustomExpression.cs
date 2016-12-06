using System;
using System.Collections.Generic;
using System.Linq;
using Cast.Util;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Languages;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_CUSTOM_EXPRESSION")]
    internal class PortfolioCustomExpression : TextBlock
    {
        #region METHODS

        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string _metricFormat = options.GetOption("FORMAT", "N2");
            string _params = options.GetOption("PARAMS", string.Empty);
            string _expr = options.GetOption("EXPR",string.Empty);
            string _aggregator = options.GetOption("AGGREGATOR", "AVERAGE");

            string[] lstParams = _params.Split(' ');
            string strParameters = string.Empty;
            object[] objValues = new object[lstParams.Length / 2];

            if (_params == string.Empty) return Labels.NoData;
            if (reportData?.Applications == null) return Labels.NoData;
            Application[] _allApps = reportData.Applications;
            string[] strValues = new string[_allApps.Length];

            for (int k = 0; k < _allApps.Length; k++)
            {
                Application _app = _allApps[k];

                Snapshot currentSnap = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                if (currentSnap == null) continue;
                int j = 0;
                for (int i = 0; i < lstParams.Length; i += 2)
                {
                    if (i == 0)
                    {
                        strParameters = "double " + lstParams[i + 1];
                    }
                    else
                    {
                        strParameters = strParameters + ", double " + lstParams[i + 1];
                    }

                    switch (lstParams[i])
                    {
                        case "SZ":
                            int sizingId = int.Parse(options.GetOption(lstParams[i + 1], "0"));
                            if (sizingId == 0)
                            {
                                objValues[j] = Labels.NoData;
                                break;
                            }
                            var sizingValue = MeasureUtility.GetSizingMeasure(currentSnap, sizingId);
                            if (sizingValue != null)
                                objValues[j] = sizingValue;
                            else
                                objValues[j] = Labels.NoData;
                            j++;
                            break;

                        case "QR":
                            int qrId = int.Parse(options.GetOption(lstParams[i + 1], "0"));
                            if (qrId == 0)
                            {
                                objValues[j] = Labels.NoData;
                                break;
                            }
                            var qrGrade = BusinessCriteriaUtility.GetMetricValue(currentSnap, qrId);
                            if (qrGrade != null)
                                objValues[j] = qrGrade;
                            else
                                objValues[j] = Labels.NoData;
                            j++;
                            break;

                        case "BF":
                            string bfId = options.GetOption(lstParams[i + 1], string.Empty);
                            if (bfId == string.Empty)
                            {
                                objValues[j] = Labels.NoData;
                                break;
                            }
                            var bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(currentSnap.Href, bfId).FirstOrDefault();
                            if (bfValue != null && bfValue.ApplicationResults.Any())
                            {
                                objValues[j] = bfValue.ApplicationResults[0].DetailResult.Value;
                            }
                            else
                            {
                                objValues[j] = Labels.NoData;
                            }
                            j++;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                string value = ExpressionEvaluator.Eval(strParameters, _expr, objValues, string.Empty);
                if (!value.Contains("Error"))
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
