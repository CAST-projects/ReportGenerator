using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Table
{
    [Block("GENERIC_TABLE")]
    public class GenericTable : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            // true for table component
            return GenericContent.Content(reportData, options, true);
        }
    }
}
