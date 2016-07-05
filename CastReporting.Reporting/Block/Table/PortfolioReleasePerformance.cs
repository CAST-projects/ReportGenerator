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
            #region Item BF
            if (options != null && options.ContainsKey("BF"))
            {
                strBackgroundFacts = options["BF"];
            }
            #endregion Item BF


            TableDefinition resultTable = null; 
            List<string> rowData = new List<string>();

            List<double> TagIds = null;

            if (strBackgroundFacts != "")
            {
                TagIds = strBackgroundFacts.Split(' ').Select(double.Parse).ToList();
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



                double? strCurrentBeforeArchDesignAll = 0;
                double? strCurrentBeforeRobuAll = 0;
                double? strCurrentBeforeSecuAll = 0;
                double? strCurrentBeforePerformanceAll = 0;
                double? strCurrentBeforeChangeAll = 0;
                double? strCurrentBeforeTransferAll = 0;
                double? strCurrentBeforeProgrammingAll = 0;
                double? strCurrentBeforeDocumentAll = 0;


                rowData.AddRange(new string[] { "Application Quality Measure", "Previous", "Target", "Actual", "SLA Violations" });
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

                                    double? strCurrentArchDesign = currSnapshotBisCriDTO.ArchitecturalDesign.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.ArchitecturalDesign.Value) : 0;
                                    double? strCurrentRobu = currSnapshotBisCriDTO.Robustness.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Robustness.Value) : 0;
                                    double? strCurrentSecu = currSnapshotBisCriDTO.Security.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Robustness.Value) : 0;
                                    double? strCurrentPerformance = currSnapshotBisCriDTO.Performance.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Performance.Value) : 0;
                                    double? strCurrentChange = currSnapshotBisCriDTO.Changeability.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Changeability.Value) : 0;
                                    double? strCurrentTransfer = currSnapshotBisCriDTO.Transferability.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Transferability.Value) : 0;
                                    double? strCurrentProgramming = currSnapshotBisCriDTO.ProgrammingPractices.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.ProgrammingPractices.Value) : 0;
                                    double? strCurrentDocument = currSnapshotBisCriDTO.Documentation.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Documentation.Value) : 0;

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

                int intFlag = 0;
                for (int j = 0; j < AllApps.Count(); j++)
                {
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
                                    double? strPreviousSecu = prevSnapshotBisCriDTO.Security.HasValue ? MathUtility.GetRound(prevSnapshotBisCriDTO.Robustness.Value) : 0;
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

                                    break; 
                                }
                            }
                        }
                    }
                }

                int intRecord = 0;
                if (intFlag == 0) // no snapshot found in the previous quarter
                {
                    for (int j = 0; j < AllApps.Count(); j++)
                    {
                        Application App = AllApps[j];

                        int nbSnapshotsEachApp = App.Snapshots.Count();
                        if (nbSnapshotsEachApp > 0)
                        {
                            foreach (Snapshot snapshot in App.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot))
                            {
                                if (intRecord == 0)
                                {
                                    intRecord = 1;
                                    continue;
                                }
                                Snapshot[] BuiltSnapshots = reportData.snapshots;

                                foreach (Snapshot BuiltSnapshot in BuiltSnapshots)
                                {
                                    if (snapshot == BuiltSnapshot)
                                    {
                                        BusinessCriteriaDTO currSnapshotBisCriDTO = BusinessCriteriaUtility.GetBusinessCriteriaGradesSnapshot(BuiltSnapshot, false);

                                        double? strCurrentBeforeArchDesign = currSnapshotBisCriDTO.ArchitecturalDesign.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.ArchitecturalDesign.Value) : 0;
                                        double? strCurrentBeforeRobu = currSnapshotBisCriDTO.Robustness.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Robustness.Value) : 0;
                                        double? strCurrentBeforeSecu = currSnapshotBisCriDTO.Security.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Robustness.Value) : 0;
                                        double? strCurrentBeforePerformance = currSnapshotBisCriDTO.Performance.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Performance.Value) : 0;
                                        double? strCurrentBeforeChange = currSnapshotBisCriDTO.Changeability.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Changeability.Value) : 0;
                                        double? strCurrentBeforeTransfer = currSnapshotBisCriDTO.Transferability.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Transferability.Value) : 0;
                                        double? strCurrentBeforeProgramming = currSnapshotBisCriDTO.ProgrammingPractices.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.ProgrammingPractices.Value) : 0;
                                        double? strCurrentBeforeDocument = currSnapshotBisCriDTO.Documentation.HasValue ? MathUtility.GetRound(currSnapshotBisCriDTO.Documentation.Value) : 0;

                                        strCurrentBeforeArchDesignAll = strCurrentBeforeArchDesignAll + strCurrentBeforeArchDesign;
                                        strCurrentBeforeRobuAll = strCurrentBeforeRobuAll + strCurrentBeforeRobu;
                                        strCurrentBeforeSecuAll = strCurrentBeforeSecuAll + strCurrentBeforeSecu;
                                        strCurrentBeforePerformanceAll = strCurrentBeforePerformanceAll + strCurrentBeforePerformance;
                                        strCurrentBeforeChangeAll = strCurrentBeforeChangeAll + strCurrentBeforeChange;
                                        strCurrentBeforeTransferAll = strCurrentBeforeTransferAll + strCurrentBeforeTransfer;
                                        strCurrentBeforeProgrammingAll = strCurrentBeforeProgrammingAll + strCurrentBeforeProgramming;
                                        strCurrentBeforeDocumentAll = strCurrentBeforeDocumentAll + strCurrentBeforeDocument;



                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                if (strCurrentBeforeArchDesignAll > 0)
                {
                    strCurrentBeforeArchDesignAll = MathUtility.GetRound(strCurrentBeforeArchDesignAll / AllApps.Count());
                }

                if (strCurrentBeforeRobuAll > 0)
                {
                    strCurrentBeforeRobuAll = MathUtility.GetRound(strCurrentBeforeRobuAll / AllApps.Count());
                }

                if (strCurrentBeforeSecuAll > 0)
                {
                    strCurrentBeforeSecuAll = MathUtility.GetRound(strCurrentBeforeSecuAll / AllApps.Count());
                }

                if (strCurrentBeforePerformanceAll > 0)
                {
                    strCurrentBeforePerformanceAll = MathUtility.GetRound(strCurrentBeforePerformanceAll / AllApps.Count());
                }

                if (strCurrentBeforeChangeAll > 0)
                {
                    strCurrentBeforeChangeAll = MathUtility.GetRound(strCurrentBeforeChangeAll / AllApps.Count());
                }

                if (strCurrentBeforeTransferAll > 0)
                {
                    strCurrentBeforeTransferAll = MathUtility.GetRound(strCurrentBeforeTransferAll / AllApps.Count());
                }

                if (strCurrentBeforeProgrammingAll > 0)
                {
                    strCurrentBeforeProgrammingAll = MathUtility.GetRound(strCurrentBeforeProgrammingAll / AllApps.Count());
                }

                if (strCurrentBeforeDocumentAll > 0)
                {
                    strCurrentBeforeDocumentAll = MathUtility.GetRound(strCurrentBeforeDocumentAll / AllApps.Count());
                }





                if (strPreviousArchDesignAll > 0)
                {
                    strPreviousArchDesignAll = MathUtility.GetRound(strPreviousArchDesignAll / AllApps.Count());
                }

                if (strPreviousRobuAll > 0)
                {
                    strPreviousRobuAll = MathUtility.GetRound(strPreviousRobuAll / AllApps.Count());
                }

                if (strPreviousSecuAll > 0)
                {
                    strPreviousSecuAll = MathUtility.GetRound(strPreviousSecuAll / AllApps.Count());
                }

                if (strPreviousPerformanceAll > 0)
                {
                    strPreviousPerformanceAll = MathUtility.GetRound(strPreviousPerformanceAll / AllApps.Count());
                }

                if (strPreviousChangeAll > 0)
                {
                    strPreviousChangeAll = MathUtility.GetRound(strPreviousChangeAll / AllApps.Count());
                }

                if (strPreviousTransferAll > 0)
                {
                    strPreviousTransferAll = MathUtility.GetRound(strPreviousTransferAll / AllApps.Count());
                }

                if (strPreviousProgrammingAll > 0)
                {
                    strPreviousProgrammingAll = MathUtility.GetRound(strPreviousProgrammingAll / AllApps.Count());
                }

                if (strPreviousDocumentAll > 0)
                {
                    strPreviousDocumentAll = MathUtility.GetRound(strPreviousDocumentAll / AllApps.Count());
                }




                if (TagIds.Count == 10)
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

                    double? lower = Math.Round((double)TagIds[0] / 100, 2);
                    double? upper = Math.Round((double)TagIds[1] / 100, 2);

                    intArchitectureSLAViol = ((TagIds[2] - MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count()))) / TagIds[2] > upper ? 0 : ((TagIds[2] - MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count()))) / TagIds[2] > lower ? 50 : 100));
                    intRobustnessSLAViol = ((TagIds[3] - MathUtility.GetRound((strCurrentRobuAll / AllApps.Count()))) / TagIds[3] > upper ? 0 : ((TagIds[3] - MathUtility.GetRound((strCurrentRobuAll / AllApps.Count()))) / TagIds[3] > lower ? 50 : 100));
                    intSecuritySLAViol = ((TagIds[4] - MathUtility.GetRound((strCurrentSecuAll / AllApps.Count()))) / TagIds[4] > upper ? 0 : ((TagIds[4] - MathUtility.GetRound((strCurrentSecuAll / AllApps.Count()))) / TagIds[4] > lower ? 50 : 100));
                    intPerformanceSLAViol = ((TagIds[5] - MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count()))) / TagIds[5] > upper ? 0 : ((TagIds[5] - MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count()))) / TagIds[5] > lower ? 50 : 100));
                    intChangeabilitySLAViol = ((TagIds[6] - MathUtility.GetRound((strCurrentChangeAll / AllApps.Count()))) / TagIds[6] > upper ? 0 : ((TagIds[6] - MathUtility.GetRound((strCurrentChangeAll / AllApps.Count()))) / TagIds[6] > lower ? 50 : 100));
                    intTransferabilitySLAViol = ((TagIds[7] - MathUtility.GetRound((strCurrentTransferAll / AllApps.Count()))) / TagIds[7] > upper ? 0 : ((TagIds[7] - MathUtility.GetRound((strCurrentTransferAll / AllApps.Count()))) / TagIds[7] > lower ? 50 : 100));
                    intProgrammingPracticeSLAViol = ((TagIds[8] - MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count()))) / TagIds[8] > upper ? 0 : ((TagIds[8] - MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count()))) / TagIds[8] > lower ? 50 : 100));
                    intDocumentationSLAViol = ((TagIds[9] - MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count()))) / TagIds[9] > upper ? 0 : ((TagIds[9] - MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count()))) / TagIds[9] > lower ? 50 : 100));


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

                    if (intFlag == 1)//TagIds[15]
                    {
                        rowData.AddRange(new string[] { "Architecture/Design", strPreviousArchDesignAll.ToString(), TagIds[2].ToString(), MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count())).ToString(), ArchitectureSLAViol });
                        rowData.AddRange(new string[] { "Robustness", strPreviousRobuAll.ToString(), TagIds[3].ToString(), MathUtility.GetRound((strCurrentRobuAll / AllApps.Count())).ToString(), RobustnessSLAViol });
                        rowData.AddRange(new string[] { "Security", strPreviousSecuAll.ToString(), TagIds[4].ToString(), MathUtility.GetRound((strCurrentSecuAll / AllApps.Count())).ToString(), SecuritySLAViol });
                        rowData.AddRange(new string[] { "Performance", strPreviousPerformanceAll.ToString(), TagIds[5].ToString(), MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count())).ToString(), PerformanceSLAViol });
                        rowData.AddRange(new string[] { "Changeability", strPreviousChangeAll.ToString(), TagIds[6].ToString(), MathUtility.GetRound((strCurrentChangeAll / AllApps.Count())).ToString(), ChangeabilitySLAViol });
                        rowData.AddRange(new string[] { "Transferability", strPreviousTransferAll.ToString(), TagIds[7].ToString(), MathUtility.GetRound((strCurrentTransferAll / AllApps.Count())).ToString(), TransferabilitySLAViol });
                        rowData.AddRange(new string[] { "Programming Practice", strPreviousProgrammingAll.ToString(), TagIds[8].ToString(), MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count())).ToString(), ProgrammingPracticeSLAViol });
                        rowData.AddRange(new string[] { "Documentation", strPreviousDocumentAll.ToString(), TagIds[9].ToString(), MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count())).ToString(), DocumentationSLAViol });
                    }
                    else
                    {
                        rowData.AddRange(new string[] { "Architecture/Design", strCurrentBeforeArchDesignAll.ToString(), TagIds[2].ToString(), MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count())).ToString(), ArchitectureSLAViol });
                        rowData.AddRange(new string[] { "Robustness", strCurrentBeforeRobuAll.ToString(), TagIds[3].ToString(), MathUtility.GetRound((strCurrentRobuAll / AllApps.Count())).ToString(), RobustnessSLAViol });
                        rowData.AddRange(new string[] { "Security", strCurrentBeforeSecuAll.ToString(), TagIds[4].ToString(), MathUtility.GetRound((strCurrentSecuAll / AllApps.Count())).ToString(), SecuritySLAViol });
                        rowData.AddRange(new string[] { "Performance", strCurrentBeforePerformanceAll.ToString(), TagIds[5].ToString(), MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count())).ToString(), PerformanceSLAViol });
                        rowData.AddRange(new string[] { "Changeability", strCurrentBeforeChangeAll.ToString(), TagIds[6].ToString(), MathUtility.GetRound((strCurrentChangeAll / AllApps.Count())).ToString(), ChangeabilitySLAViol });
                        rowData.AddRange(new string[] { "Transferability", strCurrentBeforeTransferAll.ToString(), TagIds[7].ToString(), MathUtility.GetRound((strCurrentTransferAll / AllApps.Count())).ToString(), TransferabilitySLAViol });
                        rowData.AddRange(new string[] { "Programming Practice", strCurrentBeforeProgrammingAll.ToString(), TagIds[8].ToString(), MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count())).ToString(), ProgrammingPracticeSLAViol });
                        rowData.AddRange(new string[] { "Documentation", strCurrentBeforeDocumentAll.ToString(), TagIds[9].ToString(), MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count())).ToString(), DocumentationSLAViol });
                    }
                }
                else
                {
                    rowData.AddRange(new string[] { "Architecture/Design", " ", " ", " ", MathUtility.GetRound((strCurrentArchDesignAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Robustness", " ", " ", " ", MathUtility.GetRound((strCurrentRobuAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Security", " ", " ", " ", MathUtility.GetRound((strCurrentSecuAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Performance", " ", " ", " ", MathUtility.GetRound((strCurrentPerformanceAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Changeability", " ", " ", " ", MathUtility.GetRound((strCurrentChangeAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Transferability", " ", " ", " ", MathUtility.GetRound((strCurrentTransferAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Programming Practice", " ", " ", " ", MathUtility.GetRound((strCurrentProgrammingAll / AllApps.Count())).ToString() });
                    rowData.AddRange(new string[] { "Documentation", " ", " ", " ", MathUtility.GetRound((strCurrentDocumentAll / AllApps.Count())).ToString() });
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
