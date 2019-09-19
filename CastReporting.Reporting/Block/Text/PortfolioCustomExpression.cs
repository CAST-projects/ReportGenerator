using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using CastReporting.Reporting.Languages;
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
            string _expr = options.GetOption("EXPR", string.Empty);
            string _aggregator = options.GetOption("AGGREGATOR", "AVERAGE");

            string[] lstParams = _params.Split(' ');

            if (string.IsNullOrEmpty(_params)) return Labels.NoData;
            if (reportData?.Applications == null) return Labels.NoData;
            Application[] _allApps = reportData.Applications;
            string[] strValues = new string[_allApps.Length];

            for (int k = 0; k < _allApps.Length; k++)
            {
                Application _app = _allApps[k];
                Snapshot currentSnap = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                if (currentSnap == null) continue;

                string value = MetricsUtility.CustomExpressionEvaluation(reportData, options, lstParams, currentSnap, _expr, _metricFormat, null, string.Empty, true);

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
