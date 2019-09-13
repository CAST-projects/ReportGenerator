using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
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
            string[] lstParams = options.GetOption("PARAMS", string.Empty).Split(' ');
            string _expr = options.GetOption("EXPR", string.Empty);
            string _snapshot = options.GetOption("SNAPSHOT", "CURRENT");

            if (reportData?.CurrentSnapshot == null) return Labels.NoData;
            if (lstParams.Length == 0) return Labels.NoData;

            if (_snapshot.Equals("PREVIOUS") && null != reportData.PreviousSnapshot)
            {
                return MetricsUtility.CustomExpressionEvaluation(reportData, options, lstParams, reportData.PreviousSnapshot, _expr, _metricFormat, false);
            }
            return MetricsUtility.CustomExpressionEvaluation(reportData, options, lstParams, reportData.CurrentSnapshot, _expr, _metricFormat, false);

        }

        #endregion METHODS
    }

}
