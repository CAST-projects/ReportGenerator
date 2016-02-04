using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System.Data;

namespace CastReporting.Reporting.Block.Table
{
	[Block("LIST_OF_ALL_VERSIONS")]
	class ListOfAllVersions : TableBlock
	{
		protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
		{
            int rowCount = 0;
            int nbLimitTop = 0;
            if (options == null || !options.ContainsKey("COUNT") || !Int32.TryParse(options["COUNT"], out nbLimitTop)) {
                nbLimitTop = 0;
            }

			List<string> rowData = new List<string>();

			rowData.AddRange(new string[] {
				Labels.SnapshotLabel,
				Labels.SnapshotDate
			});

			if (null != reportData && null != reportData.Application && null != reportData.Application.Snapshots) { 
				var dateFormat = Labels.FORMAT_LONG_DATE;
				var result = reportData.Application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot); 
				if (result != null) { 
					foreach (var snap in result) {
                        if (nbLimitTop <= 0 || rowCount < nbLimitTop) {
							rowData.Add(snap.Annotation.Version);
							rowData.Add(snap.Annotation.Date.DateSnapShot.HasValue
	                    		? snap.Annotation.Date.DateSnapShot.Value.ToString(dateFormat)
	                    		: Constants.No_Value);
							rowCount++;
						} 
					} 
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
