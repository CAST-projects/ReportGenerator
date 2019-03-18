/*
 *   Copyright (c) 2019 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using Cast.Util.Log;
using Cast.Util.Date;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_BC_RELEASE_PERFORMANCE")]
    public class PortfolioReleasePerformance : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string strTargets = options.GetOption("BF", string.Empty);
            string _strSla = options.GetOption("SLA", string.Empty);

            List<string> rowData = new List<string>();
            List<double> _tagIds = new List<double>();
            List<double> _slAs = new List<double>(); 

            if (!string.IsNullOrEmpty(strTargets))
            {
                string[] words = strTargets.Split(' ');
                _tagIds.AddRange(words.Select(word => Convert.ToDouble(word, System.Globalization.CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(_strSla))
            {
                string[] words = _strSla.Split(' ');
                _slAs.AddRange(words.Select(word => Convert.ToDouble(word, System.Globalization.CultureInfo.InvariantCulture)));
            }

            if (reportData.Applications != null && reportData.Snapshots != null)
            {
                int nbCurrentSnapshots = 0;
                int nbPreviousSnapshots = 0;

                double? strCurrentArchDesignAll = 0;
                double? strCurrentRobuAll = 0;
                double? strCurrentSecuAll = 0;
                double? strCurrentPerformanceAll = 0;
                double? strCurrentChangeAll = 0;
                double? strCurrentTransferAll = 0;
                double? strCurrentProgrammingAll = 0;
                double? strCurrentDocumentAll = 0;

                double? strPreviousArchDesignAll = 0;
                double? strPreviousRobuAll = 0;
                double? strPreviousSecuAll = 0;
                double? strPreviousPerformanceAll = 0;
                double? strPreviousChangeAll = 0;
                double? strPreviousTransferAll = 0;
                double? strPreviousProgrammingAll = 0;
                double? strPreviousDocumentAll = 0;

                rowData.AddRange(new[] { Labels.BusinessCriterionName, Labels.PreviousScore, Labels.Target, Labels.CurrentScore, Labels.SLAViolations });

                Application[] _allApps = reportData.Applications;
                foreach (Application _app in _allApps)
                {
                    try
                    {
                        Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot != null)
                        {
                            nbCurrentSnapshots = nbCurrentSnapshots + 1;
                            BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(_snapshot, false);

                            double? strCurrentArchDesign = currSnapshotBisCriDTO.ArchitecturalDesign ?? 0;
                            double? strCurrentRobu = currSnapshotBisCriDTO.Robustness ?? 0;
                            double? strCurrentSecu = currSnapshotBisCriDTO.Security ?? 0;
                            double? strCurrentPerformance = currSnapshotBisCriDTO.Performance ?? 0;
                            double? strCurrentChange = currSnapshotBisCriDTO.Changeability ?? 0;
                            double? strCurrentTransfer = currSnapshotBisCriDTO.Transferability ?? 0;
                            double? strCurrentProgramming = currSnapshotBisCriDTO.ProgrammingPractices ?? 0;
                            double? strCurrentDocument = currSnapshotBisCriDTO.Documentation ?? 0;

                            strCurrentArchDesignAll = strCurrentArchDesignAll + strCurrentArchDesign;
                            strCurrentRobuAll = strCurrentRobuAll + strCurrentRobu;
                            strCurrentSecuAll = strCurrentSecuAll + strCurrentSecu;
                            strCurrentPerformanceAll = strCurrentPerformanceAll + strCurrentPerformance;
                            strCurrentChangeAll = strCurrentChangeAll + strCurrentChange;
                            strCurrentTransferAll = strCurrentTransferAll + strCurrentTransfer;
                            strCurrentProgrammingAll = strCurrentProgrammingAll + strCurrentProgramming;
                            strCurrentDocumentAll = strCurrentDocumentAll + strCurrentDocument;
                        }

                        DateTime _dateNow = DateTime.Now;
                        int previousQuarter = DateUtil.GetPreviousQuarter(_dateNow);
                        int previousYear = DateUtil.GetPreviousQuarterYear(_dateNow);

                        Snapshot _previous = _app.Snapshots.Where(_ => _.Annotation.Date.DateSnapShot != null 
                            && ((_.Annotation.Date.DateSnapShot.Value.Year <= previousYear && DateUtil.GetQuarter(_.Annotation.Date.DateSnapShot.Value) <= previousQuarter) || (_.Annotation.Date.DateSnapShot.Value.Year < previousYear)))
                                .OrderByDescending(_ => _.Annotation.Date.DateSnapShot)
                                .First();

                        if (_previous == null) continue;
                        nbPreviousSnapshots = nbPreviousSnapshots + 1;
                        BusinessCriteriaDTO prevSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(_previous, false);

                        double? strPreviousArchDesign = prevSnapshotBisCriDTO.ArchitecturalDesign.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.ArchitecturalDesign.Value) : 0;
                        double? strPreviousRobu = prevSnapshotBisCriDTO.Robustness.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Robustness.Value) : 0;
                        double? strPreviousSecu = prevSnapshotBisCriDTO.Security.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Security.Value) : 0;
                        double? strPreviousPerformance = prevSnapshotBisCriDTO.Performance.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Performance.Value) : 0;
                        double? strPreviousChange = prevSnapshotBisCriDTO.Changeability.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Changeability.Value) : 0;
                        double? strPreviousTransfer = prevSnapshotBisCriDTO.Transferability.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Transferability.Value) : 0;
                        double? strPreviousProgramming = prevSnapshotBisCriDTO.ProgrammingPractices.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.ProgrammingPractices.Value) : 0;
                        double? strPreviousDocument = prevSnapshotBisCriDTO.Documentation.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Documentation.Value) : 0;

                        strPreviousArchDesignAll = strPreviousArchDesignAll + strPreviousArchDesign;
                        strPreviousRobuAll = strPreviousRobuAll + strPreviousRobu;
                        strPreviousSecuAll = strPreviousSecuAll + strPreviousSecu;
                        strPreviousPerformanceAll = strPreviousPerformanceAll + strPreviousPerformance;
                        strPreviousChangeAll = strPreviousChangeAll + strPreviousChange;
                        strPreviousTransferAll = strPreviousTransferAll + strPreviousTransfer;
                        strPreviousProgrammingAll = strPreviousProgrammingAll + strPreviousProgramming;
                        strPreviousDocumentAll = strPreviousDocumentAll + strPreviousDocument;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                        LogHelper.Instance.LogInfo(Labels.NoSnapshot);
                    }
                }

                strCurrentRobuAll = strCurrentRobuAll / nbCurrentSnapshots;
                strCurrentSecuAll = strCurrentSecuAll / nbCurrentSnapshots;
                strCurrentPerformanceAll = strCurrentPerformanceAll / nbCurrentSnapshots;
                strCurrentChangeAll = strCurrentChangeAll / nbCurrentSnapshots;
                strCurrentTransferAll = strCurrentTransferAll / nbCurrentSnapshots;
                strCurrentProgrammingAll = strCurrentProgrammingAll / nbCurrentSnapshots;
                strCurrentDocumentAll = strCurrentDocumentAll / nbCurrentSnapshots;
                strCurrentArchDesignAll = strCurrentArchDesignAll / nbCurrentSnapshots;

                if (_tagIds.Count == 8 && _slAs.Count == 2)
                {
                    double? lower = Math.Round(_slAs[0] / 100, 2);
                    double? upper = Math.Round(_slAs[1] / 100, 2);

                    string _robustnessSlaViol = (_tagIds[0] - strCurrentRobuAll) / _tagIds[0] > upper ? Labels.Bad : (_tagIds[0] - strCurrentRobuAll) / _tagIds[0] > lower ? Labels.Acceptable : Labels.Good;
                    string _securitySlaViol = (_tagIds[1] - strCurrentSecuAll) / _tagIds[1] > upper ? Labels.Bad : (_tagIds[1] - strCurrentSecuAll) / _tagIds[1] > lower ? Labels.Acceptable : Labels.Good;
                    string _performanceSlaViol = (_tagIds[2] - strCurrentPerformanceAll) / _tagIds[2] > upper ? Labels.Bad : (_tagIds[2] - strCurrentPerformanceAll) / _tagIds[2] > lower ? Labels.Acceptable : Labels.Good;
                    string _changeabilitySlaViol = (_tagIds[3] - strCurrentChangeAll) / _tagIds[3] > upper ? Labels.Bad : (_tagIds[3] - strCurrentChangeAll) / _tagIds[3] > lower ? Labels.Acceptable : Labels.Good;
                    string _transferabilitySlaViol = (_tagIds[4] - strCurrentTransferAll) / _tagIds[4] > upper ? Labels.Bad : (_tagIds[4] - strCurrentTransferAll) / _tagIds[4] > lower ? Labels.Acceptable : Labels.Good;
                    string _programmingPracticeSlaViol = (_tagIds[5] - strCurrentProgrammingAll) / _tagIds[5] > upper ? Labels.Bad : (_tagIds[5] - strCurrentProgrammingAll) / _tagIds[5] > lower ? Labels.Acceptable : Labels.Good;
                    string _documentationSlaViol = (_tagIds[6] - strCurrentDocumentAll) / _tagIds[6] > upper ? Labels.Bad : (_tagIds[6] - strCurrentDocumentAll) / _tagIds[6] > lower ? Labels.Acceptable : Labels.Good;
                    string _architectureSlaViol = (_tagIds[7] - strCurrentArchDesignAll) / _tagIds[7] > upper ? Labels.Bad : (_tagIds[7] - strCurrentArchDesignAll) / _tagIds[7] > lower ? Labels.Acceptable : Labels.Good;

                    if (nbPreviousSnapshots != 0)
                    {
                        strPreviousArchDesignAll = strPreviousArchDesignAll / nbPreviousSnapshots;
                        strPreviousRobuAll = strPreviousRobuAll / nbPreviousSnapshots;
                        strPreviousSecuAll = strPreviousSecuAll / nbPreviousSnapshots;
                        strPreviousPerformanceAll = strPreviousPerformanceAll / nbPreviousSnapshots;
                        strPreviousChangeAll = strPreviousChangeAll / nbPreviousSnapshots;
                        strPreviousTransferAll = strPreviousTransferAll / nbPreviousSnapshots;
                        strPreviousProgrammingAll = strPreviousProgrammingAll / nbPreviousSnapshots;
                        strPreviousDocumentAll = strPreviousDocumentAll / nbPreviousSnapshots;

                        rowData.AddRange(new[] { Labels.Robustness, strPreviousRobuAll.Value.ToString("N2"), _tagIds[0].ToString("N2"), strCurrentRobuAll.Value.ToString("N2"), _robustnessSlaViol });
                        rowData.AddRange(new[] { Labels.Security, strPreviousSecuAll.Value.ToString("N2"), _tagIds[1].ToString("N2"), strCurrentSecuAll.Value.ToString("N2"), _securitySlaViol });
                        rowData.AddRange(new[] { Labels.Efficiency, strPreviousPerformanceAll.Value.ToString("N2"), _tagIds[2].ToString("N2"), strCurrentPerformanceAll.Value.ToString("N2"), _performanceSlaViol });
                        rowData.AddRange(new[] { Labels.Changeability, strPreviousChangeAll.Value.ToString("N2"), _tagIds[3].ToString("N2"), strCurrentChangeAll.Value.ToString("N2"), _changeabilitySlaViol });
                        rowData.AddRange(new[] { Labels.Transferability, strPreviousTransferAll.Value.ToString("N2"), _tagIds[4].ToString("N2"), strCurrentTransferAll.Value.ToString("N2"), _transferabilitySlaViol });
                        rowData.AddRange(new[] { Labels.ProgrammingPractices, strPreviousProgrammingAll.Value.ToString("N2"), _tagIds[5].ToString("N2"), strCurrentProgrammingAll.Value.ToString("N2"), _programmingPracticeSlaViol });
                        rowData.AddRange(new[] { Labels.Documentation, strPreviousDocumentAll.Value.ToString("N2"), _tagIds[6].ToString("N2"), strCurrentDocumentAll.Value.ToString("N2"), _documentationSlaViol });
                        rowData.AddRange(new[] { Labels.ArchitecturalDesign, strPreviousArchDesignAll.Value.ToString("N2"), _tagIds[7].ToString("N2"), strCurrentArchDesignAll.Value.ToString("N2"), _architectureSlaViol });
                    }
                    else
                    {
                        rowData.AddRange(new[] { Labels.Robustness, Constants.No_Data, _tagIds[0].ToString("N2"), strCurrentRobuAll.Value.ToString("N2"), _robustnessSlaViol });
                        rowData.AddRange(new[] { Labels.Security, Constants.No_Data, _tagIds[1].ToString("N2"), strCurrentSecuAll.Value.ToString("N2"), _securitySlaViol });
                        rowData.AddRange(new[] { Labels.Efficiency, Constants.No_Data, _tagIds[2].ToString("N2"), strCurrentPerformanceAll.Value.ToString("N2"), _performanceSlaViol });
                        rowData.AddRange(new[] { Labels.Changeability, Constants.No_Data, _tagIds[3].ToString("N2"), strCurrentChangeAll.Value.ToString("N2"), _changeabilitySlaViol });
                        rowData.AddRange(new[] { Labels.Transferability, Constants.No_Data, _tagIds[4].ToString("N2"), strCurrentTransferAll.Value.ToString("N2"), _transferabilitySlaViol });
                        rowData.AddRange(new[] { Labels.ProgrammingPractices, Constants.No_Data, _tagIds[5].ToString("N2"), strCurrentProgrammingAll.Value.ToString("N2"), _programmingPracticeSlaViol });
                        rowData.AddRange(new[] { Labels.Documentation, Constants.No_Data, _tagIds[6].ToString("N2"), strCurrentDocumentAll.Value.ToString("N2"), _documentationSlaViol });
                        rowData.AddRange(new[] { Labels.ArchitecturalDesign, Constants.No_Data, _tagIds[7].ToString("N2"), strCurrentArchDesignAll.Value.ToString("N2"), _architectureSlaViol });
                    }
                }

            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 8 + 1,
                NbColumns = 5,
                Data = rowData
            };

            return resultTable;
        }
    }
}
