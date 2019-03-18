using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
	[Block("CRITICAL_VIOL_BY_APPLICATION")]
	public class CriticalVIolationByApplication : TableBlock
	{
	    public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
		{
			int param;
			bool showPrevious = false;
			if (reportData.PreviousSnapshot != null && null != options && options.ContainsKey("SHOW_PREVIOUS") && int.TryParse(options["SHOW_PREVIOUS"], out param)) {
				showPrevious = (param != 0);
			}

			List<string> rowData = new List<string>();

			int nbRows = 0;
			
			rowData.AddRange(new[] {
				" ",
				Labels.TQI,
				Labels.Robu,
				Labels.Efcy,
				Labels.Secu,
				Labels.Trans,
				Labels.Chang
			});
			nbRows++;

			var busCrit = new[] {
				Constants.BusinessCriteria.TechnicalQualityIndex,
				Constants.BusinessCriteria.Robustness,
				Constants.BusinessCriteria.Performance,
				Constants.BusinessCriteria.Security,
				Constants.BusinessCriteria.Transferability,
				Constants.BusinessCriteria.Changeability
			};
            var curVersion = new int[busCrit.Length];
            var added = new int[busCrit.Length];
            var removed = new int[busCrit.Length];

            for (int i = 0; i < busCrit.Length; i++)
            {
                var resbc = RulesViolationUtility.GetViolStat(reportData.CurrentSnapshot, busCrit[i].GetHashCode());
                if (resbc.TotalCriticalViolations != null) curVersion[i] = resbc.TotalCriticalViolations.Value;
                if (resbc.AddedCriticalViolations != null) added[i] = resbc.AddedCriticalViolations.Value;
                if (resbc.RemovedCriticalViolations != null) removed[i] = resbc.RemovedCriticalViolations.Value;
            }

			rowData.Add(Labels.VersionCurrent);
		    rowData.AddRange(curVersion.Select(curValue => curValue.ToString("N0")));
		    nbRows++;

			rowData.Add("   " + Labels.ViolationsAdded);
		    rowData.AddRange(added.Select(addValue => FormatEvolution(addValue)));
		    nbRows++;

			rowData.Add("   " + Labels.ViolationsRemoved);
		    rowData.AddRange(removed.Select(remValue => FormatEvolution(-remValue)));
		    nbRows++;

			if (showPrevious) {
				var prevVersion = new int[busCrit.Length];
                for (int i = 0; i < busCrit.Length; i++)
                {
                    var resbc = RulesViolationUtility.GetViolStat(reportData.PreviousSnapshot, busCrit[i].GetHashCode());
                    if (resbc.TotalCriticalViolations != null) prevVersion[i] = resbc.TotalCriticalViolations.Value;
                }
                rowData.Add(Labels.VersionPrevious);
			    rowData.AddRange(prevVersion.Select(prevValue => prevValue.ToString()));
			    nbRows++;
			}
			
			var resultTable = new TableDefinition {
				HasRowHeaders = false,
				HasColumnHeaders = true,
				NbRows = nbRows,
				NbColumns = busCrit.Length + 1,
				Data = rowData
			};
			

			return resultTable;
		}
	}

	
}
