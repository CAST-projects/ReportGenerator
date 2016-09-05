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

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_BC_RELEASE_PERFORMANCE")]
    class PortfolioReleasePerformance : TableBlock
    {
        private const string _MetricFormat = "N0";


        private static int GetQuarter(DateTime dt)
        {
            return (dt.Month / 4) + 1;
        }


        protected override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            
            DataTable dtDates = new DataTable();
            dtDates.Columns.Add("Quarter", typeof(int));
            dtDates.Columns.Add("Year", typeof(int)); 
            dtDates.AcceptChanges();

            string strBackgroundFacts = "";
            string strSLA = "";
            #region Item BF
            if (options != null && options.ContainsKey("BF"))
            {
                strBackgroundFacts = options["BF"];
            }
            #endregion Item BF

            #region Item SLA
            if (options != null && options.ContainsKey("SLA"))
            {
                strSLA = options["SLA"];
            }
            #endregion Item SLA

            TableDefinition resultTable = null; 
            List<string> rowData = new List<string>();

            List<double> TagIds = null;
            List<double> SLAs = null;

            if (strBackgroundFacts != "")
            {
                TagIds = strBackgroundFacts.Split(' ').Select(double.Parse).ToList();
            }

            if (strSLA != "")
            {
                SLAs = strSLA.Split(' ').Select(double.Parse).ToList();
            }




            if (reportData.Applications != null && reportData.snapshots != null)
            {

                DateTime DateNow = DateTime.Now;
                //DateTime DateNow = Convert.ToDateTime("03 05 2014"); 
                int generateQuater = 2;
                int currentYear = DateNow.Year;
                int currentQuater = GetQuarter(DateNow);
                for (int i = generateQuater; i > 0; i--)
                {
                    dtDates.Rows.Add(currentQuater, currentYear);
                    if (--currentQuater == 0)
                    {
                        currentQuater = 4;
                        currentYear--;
                    }
                }


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


                double? strPreviousArchDesignAllT = 0;
                double? strPreviousRobuAllT = 0;
                double? strPreviousSecuAllT = 0;
                double? strPreviousPerformanceAllT = 0;
                double? strPreviousChangeAllT = 0;
                double? strPreviousTransferAllT = 0;
                double? strPreviousProgrammingAllT = 0;
                double? strPreviousDocumentAllT = 0;

                rowData.AddRange(new string[] { Labels.RuleName, Labels.Previous, Labels.Target, Labels.Achievement, Labels.WithViolations });
                Snapshot Current = null;
                Application[] AllApps = reportData.Applications;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    Application App = AllApps[j];

                    int nbSnapshotsEachApp = App.Snapshots.Count();
                    if (nbSnapshotsEachApp > 0)
                    {
                        foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                        {
                            Snapshot[] BuiltSnapshots = reportData.snapshots;

                            foreach (Snapshot BuiltSnapshot in BuiltSnapshots)
                            {
                                if (snapshot == BuiltSnapshot)
                                {
                                    Current = BuiltSnapshot;
                                    BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(BuiltSnapshot, false);

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
                                     

                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                // For Previous quarter last snapshot OR if snapshot in last quarter just take the last snapshot in the series
                int RemoveApp = 0;
                for (int j = 0; j < AllApps.Count(); j++)
                {
                    strPreviousArchDesignAllT = 0;
                    strPreviousRobuAllT = 0;
                    strPreviousSecuAllT = 0;
                    strPreviousPerformanceAllT = 0;
                    strPreviousChangeAllT = 0;
                    strPreviousTransferAllT = 0;
                    strPreviousProgrammingAllT = 0;
                    strPreviousDocumentAllT = 0;

                    int intRecord = 0;
                    int intFlag = 0;
                    Application App = AllApps[j];

                    int nbSnapshotsEachApp = App.Snapshots.Count();
                    if (nbSnapshotsEachApp > 0)
                    {
                        foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                        {
                            DateTime SnapshotDate = Convert.ToDateTime(snapshot.Annotation.Date.DateSnapShot.Value);
                            int intQuarter = Convert.ToInt32(dtDates.Rows[1]["Quarter"]);
                            int intYear = Convert.ToInt32(dtDates.Rows[1]["Year"]);

                            int intSnapshotQuarter = GetQuarter(SnapshotDate);
                            int intSnapshotYear = SnapshotDate.Year;

                            if (intQuarter == intSnapshotQuarter && intYear == intSnapshotYear)
                            {
                                if (snapshot != Current)
                                {
                                    //snapshot exists in previous quarter
                                    intFlag = 1; //Flag to know that there was a snapshot in the previous quarter

                                    BusinessCriteriaDTO prevSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(snapshot, false);

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

                                    strPreviousArchDesignAllT = strPreviousArchDesignAllT + strPreviousArchDesign;
                                    strPreviousRobuAllT = strPreviousRobuAllT + strPreviousRobu;
                                    strPreviousSecuAllT = strPreviousSecuAllT + strPreviousSecu;
                                    strPreviousPerformanceAllT = strPreviousPerformanceAllT + strPreviousPerformance;
                                    strPreviousChangeAllT = strPreviousChangeAllT + strPreviousChange;
                                    strPreviousTransferAllT = strPreviousTransferAllT + strPreviousTransfer;
                                    strPreviousProgrammingAllT = strPreviousProgrammingAllT + strPreviousProgramming;
                                    strPreviousDocumentAllT = strPreviousDocumentAllT + strPreviousDocument;

                                    break; 
                                }
                            }
                        }

                        if (intFlag == 0)
                        {
                            int nbSnapshotsEachApp1 = App.Snapshots.Count();
                            if (nbSnapshotsEachApp1 == 1)
                            {
                                RemoveApp++;
                            }
                            else
                            {
                                if (nbSnapshotsEachApp > 0)
                                {
                                    foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                                    {
                                        if (intRecord == 0)
                                        {
                                            intRecord = 1;
                                            continue;
                                        }
                                        BusinessCriteriaDTO prevSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(snapshot, false);

                                        double? strPreviousArchDesign = prevSnapshotBisCriDTO.ArchitecturalDesign.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.ArchitecturalDesign.Value) : 0;
                                        double? strPreviousRobu = prevSnapshotBisCriDTO.Robustness.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Robustness.Value) : 0;
                                        double? strPreviousSecu = prevSnapshotBisCriDTO.Security.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Security.Value) : 0;
                                        double? strPreviousPerformance = prevSnapshotBisCriDTO.Performance.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Performance.Value) : 0;
                                        double? strPreviousChange = prevSnapshotBisCriDTO.Changeability.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Changeability.Value) : 0;
                                        double? strPreviousTransfer = prevSnapshotBisCriDTO.Transferability.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Transferability.Value) : 0;
                                        double? strPreviousProgramming = prevSnapshotBisCriDTO.ProgrammingPractices.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.ProgrammingPractices.Value) : 0;
                                        double? strPreviousDocument = prevSnapshotBisCriDTO.Documentation.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Documentation.Value) : 0;

                                        if (strPreviousArchDesignAllT == 0)
                                        {
                                            strPreviousArchDesignAll = strPreviousArchDesignAll + strPreviousArchDesign;
                                        }
                                        if (strPreviousRobuAllT == 0)
                                        {
                                            strPreviousRobuAll = strPreviousRobuAll + strPreviousRobu;
                                        }
                                        if (strPreviousSecuAllT == 0)
                                        {
                                            strPreviousSecuAll = strPreviousSecuAll + strPreviousSecu;
                                        }
                                        if (strPreviousPerformanceAllT == 0)
                                        {
                                            strPreviousPerformanceAll = strPreviousPerformanceAll + strPreviousPerformance;
                                        }
                                        if (strPreviousChangeAllT == 0)
                                        {
                                            strPreviousChangeAll = strPreviousChangeAll + strPreviousChange;
                                        }
                                        if (strPreviousTransferAllT == 0)
                                        {
                                            strPreviousTransferAll = strPreviousTransferAll + strPreviousTransfer;
                                        }
                                        if (strPreviousProgrammingAllT == 0)
                                        {
                                            strPreviousProgrammingAll = strPreviousProgrammingAll + strPreviousProgramming;
                                        }
                                        if (strPreviousDocumentAllT == 0)
                                        {
                                            strPreviousDocumentAll = strPreviousDocumentAll + strPreviousDocument;
                                        }
                                        break;
                                    }
                                }
                            }
                        }


                    }
                }

                


                if (strPreviousArchDesignAll > 0)
                {
                    strPreviousArchDesignAll = strPreviousArchDesignAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousRobuAll > 0)
                {
                    strPreviousRobuAll = strPreviousRobuAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousSecuAll > 0)
                {
                    strPreviousSecuAll = strPreviousSecuAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousPerformanceAll > 0)
                {
                    strPreviousPerformanceAll = strPreviousPerformanceAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousChangeAll > 0)
                {
                    strPreviousChangeAll = strPreviousChangeAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousTransferAll > 0)
                {
                    strPreviousTransferAll = strPreviousTransferAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousProgrammingAll > 0)
                {
                    strPreviousProgrammingAll = strPreviousProgrammingAll / (AllApps.Count() - RemoveApp);
                }

                if (strPreviousDocumentAll > 0)
                {
                    strPreviousDocumentAll = strPreviousDocumentAll / (AllApps.Count() - RemoveApp);
                }




                if (TagIds.Count == 8 && SLAs.Count == 2)
                {
                    int intArchitectureSLAViol = 0;
                    int intRobustnessSLAViol = 0;
                    int intSecuritySLAViol = 0;
                    int intPerformanceSLAViol = 0;
                    int intChangeabilitySLAViol = 0;
                    int intTransferabilitySLAViol = 0;
                    int intProgrammingPracticeSLAViol = 0;
                    int intDocumentationSLAViol = 0;

                    //double? lower = MathUtility.GetRound(TagIds[0] / 100);
                    //double? upper = MathUtility.GetRound(TagIds[1] / 100);

                    double? lower = Math.Round((double)SLAs[0] / 100, 2);
                    double? upper = Math.Round((double)SLAs[1] / 100, 2);

                    intRobustnessSLAViol = ((TagIds[0] - (strCurrentRobuAll / AllApps.Count())) / TagIds[0] > upper ? 0 : ((TagIds[0] - (strCurrentRobuAll / AllApps.Count())) / TagIds[0] > lower ? 50 : 100));
                    intSecuritySLAViol = ((TagIds[1] - (strCurrentSecuAll / AllApps.Count())) / TagIds[1] > upper ? 0 : ((TagIds[1] - (strCurrentSecuAll / AllApps.Count())) / TagIds[1] > lower ? 50 : 100));
                    intChangeabilitySLAViol = ((TagIds[2] - (strCurrentChangeAll / AllApps.Count())) / TagIds[2] > upper ? 0 : ((TagIds[2] - (strCurrentChangeAll / AllApps.Count())) / TagIds[2] > lower ? 50 : 100));
                    intTransferabilitySLAViol = ((TagIds[3] - (strCurrentTransferAll / AllApps.Count())) / TagIds[3] > upper ? 0 : ((TagIds[3] - (strCurrentTransferAll / AllApps.Count())) / TagIds[3] > lower ? 50 : 100));
                    intProgrammingPracticeSLAViol = ((TagIds[4] - (strCurrentProgrammingAll / AllApps.Count())) / TagIds[4] > upper ? 0 : ((TagIds[4] - (strCurrentProgrammingAll / AllApps.Count())) / TagIds[4] > lower ? 50 : 100));
                    intDocumentationSLAViol = ((TagIds[5] - (strCurrentDocumentAll / AllApps.Count())) / TagIds[5] > upper ? 0 : ((TagIds[5] - (strCurrentDocumentAll / AllApps.Count())) / TagIds[5] > lower ? 50 : 100));
                    intPerformanceSLAViol = ((TagIds[6] - (strCurrentPerformanceAll / AllApps.Count())) / TagIds[6] > upper ? 0 : ((TagIds[6] - (strCurrentPerformanceAll / AllApps.Count())) / TagIds[6] > lower ? 50 : 100));
                    intArchitectureSLAViol = ((TagIds[7] - (strCurrentArchDesignAll / AllApps.Count())) / TagIds[7] > upper ? 0 : ((TagIds[7] - (strCurrentArchDesignAll / AllApps.Count())) / TagIds[7] > lower ? 50 : 100));
                    

                    //if (intFlag == 1)//TagIds[2]
                    //{
                    //    intArchitectureSLAViol = ((TagIds[2] - MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count()))) / TagIds[2] > upper ? 0 : ((TagIds[2] - MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count()))) / TagIds[2] > lower ? 50 : 100));
                    //    intRobustnessSLAViol = ((TagIds[3] - MathUtility.GetRound((strCurrentRobuAll / AllApps.Count()))) / TagIds[3] > upper ? 0 : ((TagIds[3] - MathUtility.GetRound((strCurrentRobuAll / AllApps.Count()))) / TagIds[3] > lower ? 50 : 100));
                    //    intSecuritySLAViol = ((TagIds[4] - MathUtility.GetRound((strCurrentSecuAll / AllApps.Count()))) / TagIds[4] > upper ? 0 : ((TagIds[4] - MathUtility.GetRound((strCurrentSecuAll / AllApps.Count()))) / TagIds[4] > lower ? 50 : 100));
                    //    intPerformanceSLAViol = ((TagIds[5] - MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count()))) / TagIds[5] > upper ? 0 : ((TagIds[5] - MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count()))) / TagIds[5] > lower ? 50 : 100));
                    //    intChangeabilitySLAViol = ((TagIds[6] - MathUtility.GetRound((strCurrentChangeAll / AllApps.Count()))) / TagIds[6] > upper ? 0 : ((TagIds[6] - MathUtility.GetRound((strCurrentChangeAll / AllApps.Count()))) / TagIds[6] > lower ? 50 : 100));
                    //    intTransferabilitySLAViol = ((TagIds[7] - MathUtility.GetRound((strCurrentTransferAll / AllApps.Count()))) / TagIds[7] > upper ? 0 : ((TagIds[7] - MathUtility.GetRound((strCurrentTransferAll / AllApps.Count()))) / TagIds[7] > lower ? 50 : 100));
                    //    intProgrammingPracticeSLAViol = ((TagIds[8] - MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count()))) / TagIds[8] > upper ? 0 : ((TagIds[8] - MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count()))) / TagIds[8] > lower ? 50 : 100));
                    //    intDocumentationSLAViol = ((TagIds[9] - MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count()))) / TagIds[9] > upper ? 0 : ((TagIds[9] - MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count()))) / TagIds[9] > lower ? 50 : 100));

                    //}
                    //else
                    //{
                    //    intArchitectureSLAViol = ((strCurrentBeforeArchDesignAll - MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count()))) / strCurrentBeforeArchDesignAll > upper ? 0 : ((strCurrentBeforeArchDesignAll - MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count()))) / strCurrentBeforeArchDesignAll > lower ? 50 : 100));
                    //    intRobustnessSLAViol = ((strCurrentBeforeRobuAll - MathUtility.GetRound((strCurrentRobuAll / AllApps.Count()))) / strCurrentBeforeRobuAll > upper ? 0 : ((strCurrentBeforeRobuAll - MathUtility.GetRound((strCurrentRobuAll / AllApps.Count()))) / strCurrentBeforeRobuAll > lower ? 50 : 100));
                    //    intSecuritySLAViol = ((strCurrentBeforeSecuAll - MathUtility.GetRound((strCurrentSecuAll / AllApps.Count()))) / strCurrentBeforeSecuAll > upper ? 0 : ((strCurrentBeforeSecuAll - MathUtility.GetRound((strCurrentSecuAll / AllApps.Count()))) / strCurrentBeforeSecuAll > lower ? 50 : 100));
                    //    intPerformanceSLAViol = ((strCurrentBeforePerformanceAll - MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count()))) / strCurrentBeforePerformanceAll > upper ? 0 : ((strCurrentBeforePerformanceAll - MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count()))) / strCurrentBeforePerformanceAll > lower ? 50 : 100));
                    //    intChangeabilitySLAViol = ((strCurrentBeforeChangeAll - MathUtility.GetRound((strCurrentChangeAll / AllApps.Count()))) / strCurrentBeforeChangeAll > upper ? 0 : ((strCurrentBeforeChangeAll - MathUtility.GetRound((strCurrentChangeAll / AllApps.Count()))) / strCurrentBeforeChangeAll > lower ? 50 : 100));
                    //    intTransferabilitySLAViol = ((strCurrentBeforeTransferAll - MathUtility.GetRound((strCurrentTransferAll / AllApps.Count()))) / strCurrentBeforeTransferAll > upper ? 0 : ((strCurrentBeforeTransferAll - MathUtility.GetRound((strCurrentTransferAll / AllApps.Count()))) / strCurrentBeforeTransferAll > lower ? 50 : 100));
                    //    intProgrammingPracticeSLAViol = ((strCurrentBeforeProgrammingAll - MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count()))) / strCurrentBeforeProgrammingAll > upper ? 0 : ((strCurrentBeforeProgrammingAll - MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count()))) / strCurrentBeforeProgrammingAll > lower ? 50 : 100));
                    //    intDocumentationSLAViol = ((strCurrentBeforeDocumentAll - MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count()))) / strCurrentBeforeDocumentAll > upper ? 0 : ((strCurrentBeforeDocumentAll - MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count()))) / strCurrentBeforeDocumentAll > lower ? 50 : 100));

                    //}

                    ////

                    string ArchitectureSLAViol = "";
                    string RobustnessSLAViol = "";
                    string SecuritySLAViol = "";
                    string PerformanceSLAViol = "";
                    string ChangeabilitySLAViol = "";
                    string TransferabilitySLAViol = "";
                    string ProgrammingPracticeSLAViol = "";
                    string DocumentationSLAViol = "";

                    if (intArchitectureSLAViol == 100)
                    {
                        ArchitectureSLAViol = "Good";
                    }
                    else if (intArchitectureSLAViol == 0)
                    {
                        ArchitectureSLAViol = "Poor";
                    }
                    else
                    {
                        ArchitectureSLAViol = "Acceptable";
                    }

                    if (intRobustnessSLAViol == 100)
                    {
                        RobustnessSLAViol = "Good";
                    }
                    else if (intRobustnessSLAViol == 0)
                    {
                        RobustnessSLAViol = "Poor";
                    }
                    else
                    {
                        RobustnessSLAViol = "Acceptable";
                    }



                    if (intSecuritySLAViol == 100)
                    {
                        SecuritySLAViol = "Good";
                    }
                    else if (intSecuritySLAViol == 0)
                    {
                        SecuritySLAViol = "Poor";
                    }
                    else
                    {
                        SecuritySLAViol = "Acceptable";
                    }



                    if (intPerformanceSLAViol == 100)
                    {
                        PerformanceSLAViol = "Good";
                    }
                    else if (intPerformanceSLAViol == 0)
                    {
                        PerformanceSLAViol = "Poor";
                    }
                    else
                    {
                        PerformanceSLAViol = "Acceptable";
                    }



                    if (intChangeabilitySLAViol == 100)
                    {
                        ChangeabilitySLAViol = "Good";
                    }
                    else if (intChangeabilitySLAViol == 0)
                    {
                        ChangeabilitySLAViol = "Poor";
                    }
                    else
                    {
                        ChangeabilitySLAViol = "Acceptable";
                    }



                    if (intTransferabilitySLAViol == 100)
                    {
                        TransferabilitySLAViol = "Good";
                    }
                    else if (intTransferabilitySLAViol == 0)
                    {
                        TransferabilitySLAViol = "Poor";
                    }
                    else
                    {
                        TransferabilitySLAViol = "Acceptable";
                    }



                    if (intProgrammingPracticeSLAViol == 100)
                    {
                        ProgrammingPracticeSLAViol = "Good";
                    }
                    else if (intProgrammingPracticeSLAViol == 0)
                    {
                        ProgrammingPracticeSLAViol = "Poor";
                    }
                    else
                    {
                        ProgrammingPracticeSLAViol = "Acceptable";
                    }



                    if (intDocumentationSLAViol == 100)
                    {
                        DocumentationSLAViol = "Good";
                    }
                    else if (intDocumentationSLAViol == 0)
                    {
                        DocumentationSLAViol = "Poor";
                    }
                    else
                    {
                        DocumentationSLAViol = "Acceptable";
                    }

                    string PrevRobu = "";
                    string PrevSecu = "";
                    string PrevPerf = "";
                    string PrevChange = "";
                    string PrevTrans = "";
                    string PrevPP = "";
                    string PrevDocu = "";
                    string PrevArch = "";

                    string CurrRobu = "";
                    string CurrSecu = "";
                    string CurrPerf = "";
                    string CurrChange = "";
                    string CurrTrans = "";
                    string CurrPP = "";
                    string CurrDocu = "";
                    string CurrArch = "";

                    //string CurrRobuB = "";
                    //string CurrSecuB = "";
                    //string CurrPerfB = "";
                    //string CurrChangeB = "";
                    //string CurrTransB = "";
                    //string CurrPPB = "";
                    //string CurrDocuB = "";
                    //string CurrArchB = "";

                    PrevRobu = string.Format("{0:0.00}", strPreviousRobuAll);
                    PrevSecu = string.Format("{0:0.00}", strPreviousSecuAll);
                    PrevPerf = string.Format("{0:0.00}", strPreviousPerformanceAll);
                    PrevChange = string.Format("{0:0.00}", strPreviousChangeAll);
                    PrevTrans = string.Format("{0:0.00}", strPreviousTransferAll);
                    PrevPP = string.Format("{0:0.00}", strPreviousProgrammingAll);
                    PrevDocu = string.Format("{0:0.00}", strPreviousDocumentAll);
                    PrevArch = string.Format("{0:0.00}", strPreviousArchDesignAll);

                    CurrRobu = string.Format("{0:0.00}", strCurrentRobuAll / AllApps.Count());
                    CurrSecu = string.Format("{0:0.00}", strCurrentSecuAll / AllApps.Count());
                    CurrPerf = string.Format("{0:0.00}", strCurrentPerformanceAll / AllApps.Count());
                    CurrChange = string.Format("{0:0.00}", strCurrentChangeAll / AllApps.Count());
                    CurrTrans = string.Format("{0:0.00}", strCurrentTransferAll / AllApps.Count());
                    CurrPP = string.Format("{0:0.00}", strCurrentProgrammingAll / AllApps.Count());
                    CurrDocu = string.Format("{0:0.00}", strCurrentDocumentAll / AllApps.Count());
                    CurrArch = string.Format("{0:0.00}", strCurrentArchDesignAll / AllApps.Count());

                    string Tag0 = string.Format("{0:0.00}", TagIds[0]);
                    string Tag1 = string.Format("{0:0.00}", TagIds[1]);
                    string Tag2 = string.Format("{0:0.00}", TagIds[2]);
                    string Tag3 = string.Format("{0:0.00}", TagIds[3]);
                    string Tag4 = string.Format("{0:0.00}", TagIds[4]);
                    string Tag5 = string.Format("{0:0.00}", TagIds[5]);
                    string Tag6 = string.Format("{0:0.00}", TagIds[6]);
                    string Tag7 = string.Format("{0:0.00}", TagIds[7]);


                    rowData.AddRange(new string[] { "Robustness", PrevRobu, Tag0, CurrRobu, RobustnessSLAViol });
                    rowData.AddRange(new string[] { "Security", PrevSecu, Tag1, CurrSecu, SecuritySLAViol });
                    rowData.AddRange(new string[] { "Efficiency", PrevPerf, Tag2, CurrPerf, PerformanceSLAViol });
                    rowData.AddRange(new string[] { "Changeability", PrevChange, Tag3, CurrChange, ChangeabilitySLAViol });
                    rowData.AddRange(new string[] { "Transferability", PrevTrans, Tag4, CurrTrans, TransferabilitySLAViol });
                    rowData.AddRange(new string[] { "Programming Practice", PrevPP, Tag5, CurrPP, ProgrammingPracticeSLAViol });
                    rowData.AddRange(new string[] { "Documentation", PrevDocu, Tag6, CurrDocu, DocumentationSLAViol });
                    rowData.AddRange(new string[] { "Architecture/Design", PrevArch, Tag7, CurrArch, ArchitectureSLAViol });
                }
                else
                {
                    rowData.AddRange(new string[] { "Architecture/Design", " ", " ", " ", (strCurrentArchDesignAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Robustness", " ", " ", " ", (strCurrentRobuAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Security", " ", " ", " ", (strCurrentSecuAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Efficiency", " ", " ", " ", (strCurrentPerformanceAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Changeability", " ", " ", " ", (strCurrentChangeAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Transferability", " ", " ", " ", (strCurrentTransferAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Programming Practice", " ", " ", " ", (strCurrentProgrammingAll / AllApps.Count()).ToString() });
                    rowData.AddRange(new string[] { "Documentation", " ", " ", " ", (strCurrentDocumentAll / AllApps.Count()).ToString() });
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
