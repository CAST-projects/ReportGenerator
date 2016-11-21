/*
 *   Copyright (c) 2016 CAST
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
using CastReporting.Reporting.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using System.Data;
using Cast.Util.Log;
using Cast.Util.Date;
using CastReporting.Reporting.Helper;

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_BC_RELEASE_PERFORMANCE")]
    class PortfolioReleasePerformance : TableBlock
    {
        private const string _MetricFormat = "N0";


        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            DataTable dtDates = new DataTable();
            dtDates.Columns.Add("Quarter", typeof(int));
            dtDates.Columns.Add("Year", typeof(int)); 
            dtDates.AcceptChanges();

            string strBackgroundFacts = options.GetOption("BF", string.Empty);
            string strSLA = options.GetOption("SLA", string.Empty);

            TableDefinition resultTable = null; 
            List<string> rowData = new List<string>();
            List<double> TagIds = new List<double>();
            List<double> SLAs = new List<double>(); 

            if (strBackgroundFacts != string.Empty)
            {
                string[] words = strBackgroundFacts.Split(' ');
                foreach (string word in words)
                {
                    double result = Convert.ToDouble(word, System.Globalization.CultureInfo.InvariantCulture);
                    TagIds.Add(result);
                }
            }

            if (strSLA != string.Empty)
            {
                string[] words = strSLA.Split(' ');
                foreach (string word in words)
                {
                    double result = Convert.ToDouble(word, System.Globalization.CultureInfo.InvariantCulture);
                    SLAs.Add(result);
                }
            }

            if (reportData.Applications != null && reportData.snapshots != null)
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

                rowData.AddRange(new string[] { Labels.BusinessCriterionName, Labels.Previous, Labels.Target, Labels.Actual, Labels.SLAViolations });

                Application[] AllApps = reportData.Applications;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];

                    try
                    {
                        Snapshot _snapshot = App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                        if (_snapshot != null)
                        {
                            nbCurrentSnapshots = nbCurrentSnapshots + 1;
                            BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(_snapshot, false);

                            double? strCurrentArchDesign = currSnapshotBisCriDTO.ArchitecturalDesign.HasValue ? currSnapshotBisCriDTO.ArchitecturalDesign.Value : 0;
                            double? strCurrentRobu = currSnapshotBisCriDTO.Robustness.HasValue ? currSnapshotBisCriDTO.Robustness.Value : 0;
                            double? strCurrentSecu = currSnapshotBisCriDTO.Security.HasValue ? currSnapshotBisCriDTO.Security.Value : 0;
                            double? strCurrentPerformance = currSnapshotBisCriDTO.Performance.HasValue ? currSnapshotBisCriDTO.Performance.Value : 0;
                            double? strCurrentChange = currSnapshotBisCriDTO.Changeability.HasValue ? currSnapshotBisCriDTO.Changeability.Value : 0;
                            double? strCurrentTransfer = currSnapshotBisCriDTO.Transferability.HasValue ? currSnapshotBisCriDTO.Transferability.Value : 0;
                            double? strCurrentProgramming = currSnapshotBisCriDTO.ProgrammingPractices.HasValue ? currSnapshotBisCriDTO.ProgrammingPractices.Value : 0;
                            double? strCurrentDocument = currSnapshotBisCriDTO.Documentation.HasValue ? currSnapshotBisCriDTO.Documentation.Value : 0;

                            strCurrentArchDesignAll = strCurrentArchDesignAll + strCurrentArchDesign;
                            strCurrentRobuAll = strCurrentRobuAll + strCurrentRobu;
                            strCurrentSecuAll = strCurrentSecuAll + strCurrentSecu;
                            strCurrentPerformanceAll = strCurrentPerformanceAll + strCurrentPerformance;
                            strCurrentChangeAll = strCurrentChangeAll + strCurrentChange;
                            strCurrentTransferAll = strCurrentTransferAll + strCurrentTransfer;
                            strCurrentProgrammingAll = strCurrentProgrammingAll + strCurrentProgramming;
                            strCurrentDocumentAll = strCurrentDocumentAll + strCurrentDocument;
                        }

                        DateTime DateNow = DateTime.Now;
                        int currentQuarter = DateUtil.GetQuarter(DateNow);
                        int currentYear = DateNow.Year;

                        int previousQuarter = (currentQuarter == 1) ? 4 : currentQuarter - 1;
                        int previousYear = (currentQuarter == 1) ? currentYear - 1 : currentYear;

                        Snapshot _previous = App.Snapshots.Where(_ => _.Annotation.Date.DateSnapShot.Value.Year <= previousYear && DateUtil.GetQuarter(_.Annotation.Date.DateSnapShot.Value) <= previousQuarter)
                                                          .OrderByDescending(_ => _.Annotation.Date.DateSnapShot)
                                                          .First();

                        if (_previous != null)
                        {
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
                    }
                    catch (Exception ex)
                    {
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
                
                if (TagIds.Count == 8 && SLAs.Count == 2 && nbPreviousSnapshots != 0)
                {
                    strPreviousArchDesignAll = strPreviousArchDesignAll / nbPreviousSnapshots;
                    strPreviousRobuAll = strPreviousRobuAll / nbPreviousSnapshots;
                    strPreviousSecuAll = strPreviousSecuAll / nbPreviousSnapshots;
                    strPreviousPerformanceAll = strPreviousPerformanceAll / nbPreviousSnapshots;
                    strPreviousChangeAll = strPreviousChangeAll / nbPreviousSnapshots;
                    strPreviousTransferAll = strPreviousTransferAll / nbPreviousSnapshots;
                    strPreviousProgrammingAll = strPreviousProgrammingAll / nbPreviousSnapshots;
                    strPreviousDocumentAll = strPreviousDocumentAll / nbPreviousSnapshots;

                    double? lower = Math.Round((double)SLAs[0] / 100, 2);
                    double? upper = Math.Round((double)SLAs[1] / 100, 2);

                    string RobustnessSLAViol = (TagIds[0] - strCurrentRobuAll) / TagIds[0] > upper ? Labels.Bad : (TagIds[0] - strCurrentRobuAll) / TagIds[0] > lower ? string.Empty : Labels.Good;
                    string SecuritySLAViol = (TagIds[1] - strCurrentSecuAll) / TagIds[1] > upper ? Labels.Bad : (TagIds[1] - strCurrentSecuAll) / TagIds[1] > lower ? string.Empty : Labels.Good;
                    string ChangeabilitySLAViol = (TagIds[2] - strCurrentChangeAll) / TagIds[2] > upper ? Labels.Bad : (TagIds[2] - strCurrentChangeAll) / TagIds[2] > lower ? string.Empty : Labels.Good;
                    string TransferabilitySLAViol = (TagIds[3] - strCurrentTransferAll) / TagIds[3] > upper ? Labels.Bad : (TagIds[3] - strCurrentTransferAll) / TagIds[3] > lower ? string.Empty : Labels.Good;
                    string ProgrammingPracticeSLAViol = (TagIds[4] - strCurrentProgrammingAll) / TagIds[4] > upper ? Labels.Bad : (TagIds[4] - strCurrentProgrammingAll) / TagIds[4] > lower ? string.Empty : Labels.Good;
                    string DocumentationSLAViol = (TagIds[5] - strCurrentDocumentAll) / TagIds[5] > upper ? Labels.Bad : (TagIds[5] - strCurrentDocumentAll) / TagIds[5] > lower ? string.Empty : Labels.Good;
                    string PerformanceSLAViol = (TagIds[6] - strCurrentPerformanceAll) / TagIds[6] > upper ? Labels.Bad : (TagIds[6] - strCurrentPerformanceAll) / TagIds[6] > lower ? string.Empty : Labels.Good;
                    string ArchitectureSLAViol = (TagIds[7] - strCurrentArchDesignAll) / TagIds[7] > upper ? Labels.Bad : (TagIds[7] - strCurrentArchDesignAll) / TagIds[7] > lower ? string.Empty : Labels.Good;
                    
                    rowData.AddRange(new string[] { Labels.Robustness, strPreviousRobuAll.Value.ToString("N2"), TagIds[0].ToString("N2"), strCurrentRobuAll.Value.ToString("N2"), RobustnessSLAViol });
                    rowData.AddRange(new string[] { Labels.Security, strPreviousSecuAll.Value.ToString("N2"), TagIds[1].ToString("N2"), strCurrentSecuAll.Value.ToString("N2"), SecuritySLAViol });
                    rowData.AddRange(new string[] { Labels.Efficiency, strPreviousPerformanceAll.Value.ToString("N2"), TagIds[2].ToString("N2"), strCurrentPerformanceAll.Value.ToString("N2"), PerformanceSLAViol });
                    rowData.AddRange(new string[] { Labels.Changeability, strPreviousChangeAll.Value.ToString("N2"), TagIds[3].ToString("N2"), strCurrentChangeAll.Value.ToString("N2"), ChangeabilitySLAViol });
                    rowData.AddRange(new string[] { Labels.Transferability, strPreviousTransferAll.Value.ToString("N2"), TagIds[4].ToString("N2"), strCurrentTransferAll.Value.ToString("N2"), TransferabilitySLAViol });
                    rowData.AddRange(new string[] { Labels.ProgrammingPractices, strPreviousProgrammingAll.Value.ToString("N2"), TagIds[5].ToString("N2"), strCurrentProgrammingAll.Value.ToString("N2"), ProgrammingPracticeSLAViol });
                    rowData.AddRange(new string[] { Labels.Documentation, strPreviousDocumentAll.Value.ToString("N2"), TagIds[6].ToString("N2"), strCurrentDocumentAll.Value.ToString("N2"), DocumentationSLAViol });
                    rowData.AddRange(new string[] { Labels.ArchitecturalDesign, strPreviousArchDesignAll.Value.ToString("N2"), TagIds[7].ToString("N2"), strCurrentArchDesignAll.Value.ToString("N2"), ArchitectureSLAViol });
                }
                else
                {
                    rowData.AddRange(new string[] { Labels.Robustness, " ", " ", " ", strCurrentSecuAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.Security, " ", " ", " ", strCurrentSecuAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.Efficiency, " ", " ", " ", strCurrentPerformanceAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.Changeability, " ", " ", " ", strCurrentChangeAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.Transferability, " ", " ", " ", strCurrentTransferAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.ProgrammingPractices, " ", " ", " ", strCurrentProgrammingAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.Documentation, " ", " ", " ", strCurrentDocumentAll.Value.ToString("N2") });
                    rowData.AddRange(new string[] { Labels.ArchitecturalDesign, " ", " ", " ", strCurrentRobuAll.Value.ToString("N2") });
                }
            }

            resultTable = new TableDefinition
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
