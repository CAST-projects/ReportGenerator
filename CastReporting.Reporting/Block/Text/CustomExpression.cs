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
    [Block("CUSTOM_EXPRESSION")]
    internal class CustomExpression : TextBlock
    {
        #region METHODS

        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string _metricFormat = options.GetOption("FORMAT", "N2");
            string _params = options.GetOption("PARAMS", string.Empty);
            string _expr = options.GetOption("EXPR",string.Empty);
            string _snapshot = options.GetOption("SNAPSHOT", "CURRENT");

            string[] lstParams = _params.Split(' ');
            string strParameters = string.Empty;
            object[] objValues = new object[lstParams.Length / 2];

            if (reportData?.CurrentSnapshot == null) return Labels.NoData;
            if (_params == string.Empty) return Labels.NoData;
            int j = 0;
            for (int i=0; i < lstParams.Length; i+=2)
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
                            return Labels.NoData;
                        double? sizingValue;
                        if (_snapshot.Equals("PREVIOUS") && null != reportData.PreviousSnapshot)
                        {
                            sizingValue = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, sizingId);
                        }
                        else
                        {
                            sizingValue = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, sizingId);
                        }
                        objValues[j] = sizingValue;
                        j++;
                        break;

                    case "QR":
                        int qrId = int.Parse(options.GetOption(lstParams[i + 1], "0"));
                        if (qrId == 0)
                            return Labels.NoData;
                        double? qrGrade;
                        if (_snapshot.Equals("PREVIOUS") && null != reportData.PreviousSnapshot)
                        {
                            qrGrade = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, qrId);
                        }
                        else
                        {
                            qrGrade = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot, qrId);
                        }
                        objValues[j] = qrGrade;
                        j++;
                        break;

                    case "BF":
                        string bfId = options.GetOption(lstParams[i + 1], string.Empty);
                        if (bfId == string.Empty)
                            return Labels.NoData;
                        Result bfValue;
                        if (_snapshot.Equals("PREVIOUS") && null != reportData.PreviousSnapshot)
                        {
                            bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.PreviousSnapshot.Href, bfId).FirstOrDefault();
                        }
                        else
                        {
                            bfValue = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.CurrentSnapshot.Href, bfId).FirstOrDefault();
                        }
                        if (bfValue != null && bfValue.ApplicationResults.Any())
                        {
                            objValues[j] = bfValue.ApplicationResults[0].DetailResult.Value;
                        }
                        else
                        {
                            return Labels.NoData;
                        }
                        j++; 
                        break;
                }
            }

            string value = ExpressionEvaluator.Eval(strParameters, _expr, objValues, _metricFormat);
            return value;
        }
        #endregion METHODS
    }
}
