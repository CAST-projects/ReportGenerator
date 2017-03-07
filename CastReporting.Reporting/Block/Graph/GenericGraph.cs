﻿using System;
using System.Collections.Generic;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
using System.Linq;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing.DTO;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("GENERIC_GRAPH")]
    public class GenericGraph : GraphBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            return GenericContent.Content(reportData, options, false);
        }
    }
}
