using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Graph
{
    [Block("GENERIC_GRAPH")]
    public class GenericGraph : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            // false for graph component
            return GenericContent.Content(reportData, options, false);
         }
    }
}
