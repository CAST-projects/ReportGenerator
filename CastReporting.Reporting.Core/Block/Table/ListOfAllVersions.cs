using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
	[Block("LIST_OF_ALL_VERSIONS")]
	public class ListOfAllVersions : TableBlock
	{
	    public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
		{
            int rowCount = 0;
            int nbLimitTop;
            if (options == null || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = 0;
            }

			List<string> rowData = new List<string>();

			rowData.AddRange(new[] {
				Labels.SnapshotLabel,
				Labels.SnapshotDate
			});

			if (reportData?.Application?.Snapshots != null)
            { 
				var dateFormat = Labels.FORMAT_LONG_DATE;
				var result = reportData.Application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot);
                foreach (var snap in result)
                {
                    if (nbLimitTop > 0 && rowCount >= nbLimitTop) continue;
                    rowData.Add(snap.Annotation.Version);
                    rowData.Add(snap.Annotation.Date.DateSnapShot?.ToString(dateFormat) ?? Constants.No_Value);
                    rowCount++;
                }
            }

			return new TableDefinition {
				HasRowHeaders = false,
				HasColumnHeaders = true,
				NbRows = rowCount + 1,
				NbColumns = 2,
				Data = rowData
			};
		}
	}
}
