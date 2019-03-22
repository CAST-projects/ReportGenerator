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
    [Block("CUSTOM_EXPRESSION")]
    public class CustomExpression : TextBlock
    {
        #region METHODS

        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string _metricFormat = options.GetOption("FORMAT", "N2");
            string _params = options.GetOption("PARAMS", string.Empty);
            string _expr = options.GetOption("EXPR",string.Empty);
            string _snapshot = options.GetOption("SNAPSHOT", "CURRENT");

            string[] lstParams = _params.Split(' ');

            if (reportData?.CurrentSnapshot == null) return Labels.NoData;
            if (string.IsNullOrEmpty(_params)) return Labels.NoData;
            for (int i=0; i < lstParams.Length; i+=2)
            {
                string param = lstParams[i + 1];
                double? paramValue;

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
                        paramValue = sizingValue;
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
                        paramValue = qrGrade;
                        break;

                    case "BF":
                        string bfId = options.GetOption(lstParams[i + 1], string.Empty);
                        if (string.IsNullOrEmpty(bfId)) return Labels.NoData;
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
                            paramValue = bfValue.ApplicationResults[0].DetailResult.Value;
                        }
                        else
                        {
                            return Labels.NoData;
                        }
                        break;
                    default:
                        return Labels.NoData;
                }
                if (paramValue != null) _expr = _expr.Replace(param, paramValue.ToString());

            }
            DataTable dt = new DataTable();
            return double.Parse(dt.Compute(_expr, "").ToString()).ToString(_metricFormat);
        }
        #endregion METHODS
    }
}
