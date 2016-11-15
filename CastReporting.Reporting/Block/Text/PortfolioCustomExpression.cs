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
using CastReporting.BLL;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_CUSTOM_EXPRESSION")]
    class PortfolioCustomExpression : TextBlock
    {
        #region METHODS

        protected override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string _MetricFormat = options.GetOption("FORMAT", "N2");
            string _Params = options.GetOption("PARAMS", string.Empty);
            string _Expr = options.GetOption("EXPR",string.Empty);
            string _Aggregator = options.GetOption("AGGREGATOR", "AVERAGE");

            string[] lstParams = _Params.Split(' ');
            string strParameters = string.Empty;
            object[] objValues = new object[lstParams.Length / 2];

            if (_Params != string.Empty)
            {
                if (null != reportData && null != reportData.Applications)
                {
                    Application[] AllApps = reportData.Applications;
                    string[] strValues = new string[AllApps.Count()];

                    for (int k = 0; k < AllApps.Count(); k++)
                    {
                        Application App = AllApps[k];

                        Snapshot currentSnap = App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (currentSnap != null)
                        {
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
                                        int sizingId = Int32.Parse(options.GetOption(lstParams[i + 1], "0"));
                                        if (sizingId == 0)
                                        {
                                            objValues[j] = Labels.NoData;
                                            break;
                                        }
                                        double? sizingValue = null;
                                        sizingValue = MeasureUtility.GetSizingMeasure(currentSnap, sizingId);
                                        if (sizingValue != null)
                                            objValues[j] = sizingValue;
                                        else
                                            objValues[j] = Labels.NoData;
                                        j++;
                                        break;

                                    case "QR":
                                        int qrId = Int32.Parse(options.GetOption(lstParams[i + 1], "0"));
                                        if (qrId == 0)
                                        {
                                            objValues[j] = Labels.NoData;
                                            break;
                                        }
                                        double? qrGrade = null;
                                        qrGrade = BusinessCriteriaUtility.GetMetricValue(currentSnap, qrId);
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
                                        Result bfValue;
                                        bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(currentSnap.Href, bfId).FirstOrDefault();
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
                                }
                            }
                            string value = ExpressionEvaluator.Eval(strParameters, _Expr, objValues, string.Empty);
                            if (!value.Contains("Error"))
                            {
                                strValues[k] = value;
                            }
                            else
                            {
                                strValues[k] = Labels.NoData;
                            }
                        }

                    }


                    if (_Aggregator == "SUM")
                    {
                        double sumValues = 0;
                        for (int i = 0; i < strValues.Length; i++)
                        {
                            if (strValues[i] != Labels.NoData)
                            {
                                sumValues += Double.Parse(strValues[i]);
                            }
                        }
                        return sumValues.ToString(_MetricFormat);
                    }
                    else
                    {
                        double avgValues = 0;
                        double sumValues = 0;
                        int nbValues = 0;
                        for (int i = 0; i < strValues.Length; i++)
                        {
                            if (strValues[i] != Labels.NoData)
                            {
                                nbValues++;
                                sumValues += Double.Parse(strValues[i]);
                            }
                        }
                        if (nbValues != 0)
                        {
                            avgValues = sumValues / nbValues;
                            return avgValues.ToString(_MetricFormat);
                        }
                        else
                        {
                            return Labels.NoData;
                        }
                    }

                }
                else
                {
                    return Labels.NoData;
                }
            }
            else
            {
                return Labels.NoData;
            }
        }
        #endregion METHODS
    }
}
