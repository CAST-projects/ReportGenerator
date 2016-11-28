using System;
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
	class CriticalVIolationByApplication : TableBlock
	{
		protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
		{
			var results = RulesViolationUtility.GetStatViolation(reportData.CurrentSnapshot);

			int param = 0;
			bool showPrevious = false;
			if (reportData.PreviousSnapshot != null && null != options && options.ContainsKey("SHOW_PREVIOUS") && Int32.TryParse(options["SHOW_PREVIOUS"], out param)) {
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

			var busCrit = new Constants.BusinessCriteria[] {
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
			
			foreach (var resultModule in results) {
				if (resultModule != null) {
					for (int i = 0; i < busCrit.Length; i++) {
						var crit = busCrit[i];
                        if (resultModule[crit] != null) { 
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
				}
			}

			rowData.Add(Labels.VersionCurrent);
			foreach (var curValue in curVersion) {
				rowData.Add(curValue.ToString());
			}
			nbRows++;

			rowData.Add("   " + Labels.ViolationsAdded);
			foreach (var addValue in added) {
				rowData.Add(TableBlock.FormatEvolution(addValue));
			}
			nbRows++;

			rowData.Add("   " + Labels.ViolationsRemoved);
			foreach (var remValue in removed) {
				rowData.Add(TableBlock.FormatEvolution(-remValue));
			}
			nbRows++;

			if (showPrevious) {
				var prevVersion = new int[busCrit.Length];

				results = RulesViolationUtility.GetStatViolation(reportData.PreviousSnapshot);
				foreach (var resultModule in results) {
					if (resultModule != null) {
						for (int i = 0; i < busCrit.Length; i++) {
							var crit = busCrit[i];
							if (resultModule[crit] != null && resultModule[crit].TotalCriticalViolations.HasValue) {
								prevVersion[i] += resultModule[crit].TotalCriticalViolations.Value;
							}
						}
					}
				}

				rowData.Add(Labels.VersionPrevious);
				foreach (var prevValue in prevVersion) {
					rowData.Add(prevValue.ToString());
				}
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
