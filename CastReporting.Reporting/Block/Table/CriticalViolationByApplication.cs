using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;

namespace CastReporting.Reporting.Block.Table
{
	[Block("CRITICAL_VIOL_BY_APPLICATION")]
	internal class CriticalVIolationByApplication : TableBlock
	{
		protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
		{
			var results = RulesViolationUtility.GetStatViolation(reportData.CurrentSnapshot);

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
			
			foreach (var resultModule in results)
			{
			    if (resultModule == null) continue;
			    for (int i = 0; i < busCrit.Length; i++) {
			        var crit = busCrit[i];
			        if (resultModule[crit] == null) continue;
			        if (resultModule[crit].TotalCriticalViolations.HasValue) {
			            curVersion[i] += resultModule[crit].TotalCriticalViolations.Value;
			        }
			        if (resultModule[crit].AddedCriticalViolations.HasValue) {
			            added[i] += resultModule[crit].AddedCriticalViolations.Value;
			        }
			        if (resultModule[crit].RemovedCriticalViolations.HasValue) {
			            removed[i] += resultModule[crit].RemovedCriticalViolations.Value;
			        }
			    }
			}

			rowData.Add(Labels.VersionCurrent);
		    rowData.AddRange(curVersion.Select(curValue => curValue.ToString()));
		    nbRows++;

			rowData.Add("   " + Labels.ViolationsAdded);
		    rowData.AddRange(added.Select(addValue => FormatEvolution(addValue)));
		    nbRows++;

			rowData.Add("   " + Labels.ViolationsRemoved);
		    rowData.AddRange(removed.Select(remValue => FormatEvolution(-remValue)));
		    nbRows++;

			if (showPrevious) {
				var prevVersion = new int[busCrit.Length];

				results = RulesViolationUtility.GetStatViolation(reportData.PreviousSnapshot);
				foreach (var resultModule in results)
				{
				    if (resultModule == null) continue;
				    for (int i = 0; i < busCrit.Length; i++) {
				        var crit = busCrit[i];
				        if (resultModule[crit] != null && resultModule[crit].TotalCriticalViolations.HasValue) {
				            prevVersion[i] += resultModule[crit].TotalCriticalViolations.Value;
				        }
				    }
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
