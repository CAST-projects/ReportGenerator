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


using Cast.Util;
using Cast.Util.Log;
using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.Reporting.Builder;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Data;



namespace CastReporting.UI.WPF.ViewModel
{
    /// <summary>
    ///  Implement the ViewModel for the reporting page
    /// </summary>
    public class ReportingVM : ViewModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        public ICommand GenerateCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadTemplatesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadTagsCommand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICommand ReloadTemplatesCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadSnapshotsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand LoadPreviousSnapshotsCommand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<FileInfo> _TemplateFiles;
        public IEnumerable<FileInfo> TemplateFiles
        {
            get { return _TemplateFiles; }
            set
            {
                _TemplateFiles = value;
                base.OnPropertyChanged("TemplateFiles");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private FileInfo _SelectedTemplateFile;
        public FileInfo SelectedTemplateFile
        {
            get { return _SelectedTemplateFile; }
            set
            {
                if (value == _SelectedTemplateFile)
                    return;
                _SelectedTemplateFile = value;
                base.OnPropertyChanged("SelectedTemplateFile");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<Application> _Applications;
        public IEnumerable<Application> Applications
        {
            get { return _Applications; }
            set
            {
                _Applications = value;
                base.OnPropertyChanged("Applications");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> _Categories;
        public IEnumerable<string> Categories
        {
            get { return _Categories; }
            set
            {
                _Categories = value;
                base.OnPropertyChanged("Categories");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<string> _Tags;
        public IEnumerable<string> Tags
        {
            get { return _Tags; }
            set
            {
                _Tags = value;
                base.OnPropertyChanged("Tags");
            }
        }


      
        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<Snapshot> _Snaphosts;
        public IEnumerable<Snapshot> Snaphosts
        {
            get { return _Snaphosts; }
            set
            {
                _Snaphosts = value;
                base.OnPropertyChanged("Snaphosts");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<Snapshot> _PreviousSnapshosts { get; set; }
        public IEnumerable<Snapshot> PreviousSnaphosts
        {
            get { return _PreviousSnapshosts; }
            set
            {
                _PreviousSnapshosts = value;
                base.OnPropertyChanged("PreviousSnaphosts");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private CastDomain _SelectedDomain;
        public CastDomain SelectedDomain
        {
            get { return _SelectedDomain; }
            set
            {
                if (value == _SelectedDomain)
                    return;
                _SelectedDomain = value;
                base.OnPropertyChanged("SelectedDomain");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private Application _SelectedApplication;
        public Application SelectedApplication
        {
            get { return _SelectedApplication; }
            set
            {
                if (value == _SelectedApplication)
                    return;
                _SelectedApplication = value;

                base.OnPropertyChanged("SelectedApplication");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _SelectedCategory;
        public string SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if (value == _SelectedCategory)
                    return;
                _SelectedCategory = value;

                base.OnPropertyChanged("SelectedCategory");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Snapshot _SelectedSnapshot;
        public Snapshot SelectedSnapshot
        {
            get { return _SelectedSnapshot; }
            set
            {
                if (value == _SelectedSnapshot)
                    return;
                _SelectedSnapshot = value;

                base.OnPropertyChanged("SelectedSnapshot");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }

        private int _SelectedTab;
        public int SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                if (value == _SelectedTab)
                    return;
                _SelectedTab = value;

                base.OnPropertyChanged("SelectedTab");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _SelectedTag;
        public string SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                if (value == _SelectedTag)
                    return;
                _SelectedTag = value;

                base.OnPropertyChanged("SelectedTag");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private Snapshot _PreviousSnapshot;
        public Snapshot PreviousSnapshot
        {
            get { return _PreviousSnapshot; }
            set
            {
                if (value == _PreviousSnapshot)
                    return;
                _PreviousSnapshot = value;

                base.OnPropertyChanged("PreviousSnapshot");
                base.OnPropertyChanged("IsDataFilledIn");
            }
        }


        /// <summary>
        /// File name
        /// </summary>
        private string _ReportFileName;
        public string ReportFileName
        {
            get
            {
                return _ReportFileName;
            }

            set
            {
                if (value == _ReportFileName)
                    return;
                _ReportFileName = value;

                base.OnPropertyChanged("ReportFileName");
            }
        }

        /// <summary>
        /// 
        /// </summary>      
        public bool Is
        {
            get
            {
                if (_SelectedTag != null && _SelectedTemplateFile != null)
                {
                    return true;
                }
                else
                {
                    return _SelectedApplication != null &&
                           _SelectedSnapshot != null &&
                           _SelectedTemplateFile != null;
                }
            }
        }

        
        /// <summary>
        /// 
        /// </summary>      
        public bool IsDataFilledIn
        {
            get
            {
                //if (_SelectedTag != null && _SelectedTemplateFile != null)
                if (_SelectedTemplateFile != null && _SelectedTab == 1)
                {
                    return true;
                }
                else if (_SelectedApplication != null && _SelectedSnapshot != null && _SelectedTemplateFile != null && _SelectedTab == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>      
        //public bool IsDataFilledIn
        //{
        //    get
        //    {
        //        //if (_SelectedTag != null && _SelectedTemplateFile != null)
        //        if (_SelectedTemplateFile != null && _SelectedTab==1)
        //        {
        //            return true;
        //        }
        //        else  
        //        {
        //            return _SelectedApplication != null &&
        //                   _SelectedSnapshot != null &&
        //                   _SelectedTemplateFile != null;
        //        } 
        //    }
        //}
     
        /// <summary>
        /// 
        /// </summary>
        private WSConnection _ActiveConnection;
        public WSConnection ActiveConnection
        {
            get
            {
                return _ActiveConnection;
            }
            set
            {
                _ActiveConnection = value;

                base.OnPropertyChanged("ActiveConnection");
            }

        }

        /// <summary>
        /// Contructor
        /// </summary>
        public ReportingVM()
        {
            GenerateCommand = new CommandHandler(ExecuteGenerateCommand, CanExecuteGenerateCommand);
            LoadSnapshotsCommand = new CommandHandler(ExecuteLoadSnapshotsCommand, null);
            LoadPreviousSnapshotsCommand = new CommandHandler(ExecuteLoadPreviousSnapshotsCommand, null);
            LoadTemplatesCommand = new CommandHandler(ExecuteLoadTemplatesCommand, null);
            LoadTagsCommand = new CommandHandler(ExecuteLoadTagsCommand, null);
            ReloadTemplatesCommand = new CommandHandler(ExecuteReloadTemplatesCommand, null);
            //Load Tempalte
            LoadTemplatesCommand.Execute(null);
        }
        /// <summary>
        /// Implement Command that Load the templates list
        /// </summary>
        void ExecuteReloadTemplatesCommand(object parameter)
        {

            //if (_SelectedTag != null && _SelectedTemplateFile != null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return _SelectedApplication != null &&
            //           _SelectedSnapshot != null &&
            //           _SelectedTemplateFile != null;
            //}
            if (SelectedTab == 0)
            {
                SelectedCategory = null;
                _SelectedCategory = null;
                _SelectedTemplateFile = null;
                SelectedTemplateFile = null;
                TemplateFiles = SettingsBLL.GetTemplateFileList();
            }
            else
                if (SelectedTab == 1)
                {
                    SelectedApplication = null;
                    SelectedSnapshot = null;
                    _SelectedApplication = null;
                    _SelectedSnapshot = null;

                    
                    TemplateFiles = SettingsBLL.GetTemplateFileListPortfolio();
                }
        }
        

        /// <summary>
        /// Implement Command that Load the templates list
        /// </summary>
        void ExecuteLoadTagsCommand(object parameter)
        {
            if (SelectedCategory != null)
            {
                //GetActive Connection           
                ActiveConnection = (Setting != null) ? Setting.GetActiveConnection() : null;

                //Get list of domains
                if (_ActiveConnection != null)
                {
                    try
                    {
                        using (CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection))
                        {
                            Tags = castDomainBLL.GetTags(SelectedCategory);
                        }
                    }
                    catch (Exception ex)
                    {
                        base.MessageManager.OnErrorOccured(ex);
                    }
                }
            }
            else

                Tags = null;
        }

        /// <summary>
        /// Implement Command that Load the templates list
        /// </summary>
        void ExecuteLoadTemplatesCommand(object parameter)
        {
            if (SelectedTab == 0)
            {
                TemplateFiles = SettingsBLL.GetTemplateFileList();
            }
            else
                if (SelectedTab == 1)
                {
                    TemplateFiles = SettingsBLL.GetTemplateFileListPortfolio();
                }
        }

        /// <summary>
        /// Implement Command that Load the templates list of Portfolio
        /// </summary>
        void ExecuteLoadTemplatesPortfolioCommand(object parameter)
        {
            TemplateFiles = SettingsBLL.GetTemplateFileListPortfolio();
        }

        /// <summary>
        /// Implement Command that Load the current snapshots list
        /// </summary>
        void ExecuteLoadSnapshotsCommand(object parameter)
        {
            if (SelectedApplication != null)
            {               
                using (ApplicationBLL applicationBLL = new ApplicationBLL(ActiveConnection, SelectedApplication))
                {
                    applicationBLL.SetSnapshots();
                    Snaphosts = SelectedApplication.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).ToList();
                }
            }
            else
                Snaphosts = null;
        }


        /// <summary>
        /// Implement Command that Load previous snapshots list
        /// </summary>
        void ExecuteLoadPreviousSnapshotsCommand(object parameter)
        {
            if (SelectedSnapshot != null && Snaphosts != null)
                PreviousSnaphosts = Snaphosts.Where(_ => _.Annotation.Date.CompareTo(SelectedSnapshot.Annotation.Date) < 0).ToList();
            else
                PreviousSnaphosts = null;
        }


        /// <summary>
        /// Implement Command that Launch the report generation 
        /// </summary>

        void ExecuteGenerateCommand(object prameter)
        {
            if (!string.IsNullOrEmpty(ReportFileName))
            {
                BackgroundWorker BackgroundWorker = new BackgroundWorker();

                BackgroundWorker.DoWork += BackgroundWorkerDoWork;

               
                BackgroundWorker.RunWorkerAsync();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool CanExecuteGenerateCommand(object param)
        {
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (SelectedTab == 0)
            {
                double progressStep = 100 / 4;
                Stopwatch stopWatchStep = new Stopwatch();
                Stopwatch stopWatchGlobal = new Stopwatch();

                try
                {
                    stopWatchGlobal.Start();
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(base.MessageManager.SetBusyMode), true);

                    //Set culture for the new thread
                    if (!string.IsNullOrEmpty(Setting.ReportingParameter.CultureName))
                    {
                        var culture = new CultureInfo(Setting.ReportingParameter.CultureName);
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                    }

                    //Get result for the Application               
                    stopWatchStep.Restart();
                    ApplicationBLL.BuildApplicationResult(ActiveConnection, SelectedApplication);
                    stopWatchStep.Stop();
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Build result for the application", stopWatchStep.Elapsed);


                    //Get result for the previous snapshot                
                    stopWatchStep.Restart();
                    SnapshotBLL.BuildSnapshotResult(ActiveConnection, SelectedSnapshot, true);
                    stopWatchStep.Stop();
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Build result for the selected snapshot", stopWatchStep.Elapsed);


                    //Get result for the previuos snapshot                
                    if (PreviousSnapshot != null)
                    {
                        stopWatchStep.Restart();
                        SnapshotBLL.BuildSnapshotResult(ActiveConnection, PreviousSnapshot, false);
                        stopWatchStep.Stop();

                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Build result for the previous snapshot", stopWatchStep.Elapsed);
                    }

                    //Launch generaion               
                    stopWatchStep.Restart();
                    GenerateReport();
                    stopWatchStep.Stop();

                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Report generated", stopWatchStep.Elapsed);


                    //Show final message and unlock the screen   
                    stopWatchGlobal.Stop();
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string, TimeSpan>(base.MessageManager.OnReportGenerated), ReportFileName, stopWatchGlobal.Elapsed);
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(base.MessageManager.SetBusyMode), false);
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<Exception>(WorkerThreadException), ex);
                }
            }
            else
            {
                List<Application> Apps = new List<Application>();
                List<Snapshot> Snapshots = new List<Snapshot>();
                //if (SelectedTag != null)
                //{
                    //GetActive Connection           
                    ActiveConnection = (Setting != null) ? Setting.GetActiveConnection() : null;

                    //Get list of domains
                    if (_ActiveConnection != null)
                    {
                        try
                        {
                            using (CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection))
                            {
                                Apps = castDomainBLL.GetCommonTaggedApplications(SelectedTag);
                            }
                        }
                        catch (Exception ex)
                        {
                            base.MessageManager.OnErrorOccured(ex);
                        }
                    }
                

                if (Apps != null)
                {
                    Application[] SelectedApps = Apps.ToArray<Application>(); 

                    double progressStep = 100 / 4;
                    Stopwatch stopWatchStep = new Stopwatch();
                    Stopwatch stopWatchGlobal = new Stopwatch();

                    try
                    {
                        stopWatchGlobal.Start();
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(base.MessageManager.SetBusyMode), true);

                        //Set culture for the new thread
                        if (!string.IsNullOrEmpty(Setting.ReportingParameter.CultureName))
                        {
                            var culture = new CultureInfo(Setting.ReportingParameter.CultureName);
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = culture;
                        }
                        string[] SnapsToIgnore = null;
                        //Get result for the Portfolio               
                        stopWatchStep.Restart();
                        string[] AppsToIgnorePortfolioResult = PortfolioBLL.BuildPortfolioResult(ActiveConnection, SelectedApps);
                        stopWatchStep.Stop();
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Build result for the portfolio", stopWatchStep.Elapsed);
                        
                        List<Application> N_Apps = new List<Application>();
                        //Remove from Array the Ignored Apps
                        for (int i = 0; i < SelectedApps.Count(); i++)
                        {
                            int intAppYes = 0;
                            foreach (string s in AppsToIgnorePortfolioResult)
                            {
                                if (s == SelectedApps[i].Name)
                                {
                                    intAppYes = 1;
                                    break;
                                }
                                else
                                {
                                    intAppYes = 0;
                                }
                            }

                            if (intAppYes == 0)
                            {
                                N_Apps.Add(SelectedApps[i]);
                            }
                        }

                        Application[] N_SelectedApps = N_Apps.ToArray();

                            //GetActive Connection           
                            ActiveConnection = (Setting != null) ? Setting.GetActiveConnection() : null;

                        //Get list of domains
                        if (_ActiveConnection != null)
                        {
                            try
                            {
                                using (CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection))
                                {
                                    Snapshots = castDomainBLL.GetAllSnapshots(N_SelectedApps);
                                }
                            }
                            catch (Exception ex)
                            {
                                base.MessageManager.OnErrorOccured(ex);
                            }
                        }
                        List<Snapshot> N_Snaps = new List<Snapshot>();
                        //Get result for each app's latest snapshot
                        if (Snapshots != null)
                        {
                            Snapshot[] SelectedApps_Snapshots = Snapshots.ToArray<Snapshot>();

                            //Get result for all snapshots in Portfolio               
                            stopWatchStep.Restart();
                            SnapsToIgnore = PortfolioSnapshotsBLL.BuildSnapshotResult(ActiveConnection, SelectedApps_Snapshots, true);
                            stopWatchStep.Stop();
                            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Build result for snapshots in portfolio", stopWatchStep.Elapsed);

                            for (int i = 0; i < SelectedApps_Snapshots.Count(); i++)
                            {
                                int intRemoveYes = 0;
                                foreach (string s in SnapsToIgnore)
                                {
                                    if (s == SelectedApps_Snapshots[i].Href)
                                    {
                                        intRemoveYes = 1;
                                        break;
                                    }
                                    else
                                    {
                                        intRemoveYes = 0;
                                    }
                                }
                                if (intRemoveYes == 0)
                                {
                                    N_Snaps.Add(SelectedApps_Snapshots[i]);
                                }
                            }

                            Snapshot[] N_SelectedApps_Snapshots = N_Snaps.ToArray();


                                //Launch generaion               
                                stopWatchStep.Restart();
                                GenerateReportPortfolio(N_SelectedApps, N_SelectedApps_Snapshots, AppsToIgnorePortfolioResult, SnapsToIgnore);
                            stopWatchStep.Stop();
                        }





                        if ((AppsToIgnorePortfolioResult.Count() > 0 && AppsToIgnorePortfolioResult != null) || (SnapsToIgnore.Count() > 0 && SnapsToIgnore != null))
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("Some Applications or Snapshots were ignored during processing REST API.");

                            if (AppsToIgnorePortfolioResult.Count() > 0 && AppsToIgnorePortfolioResult != null)
                            {
                                AppsToIgnorePortfolioResult = AppsToIgnorePortfolioResult.Distinct().ToArray();
                                sb.Append("Ignored Applications are: ");
                                for (int i = 0; i < AppsToIgnorePortfolioResult.Count(); i++)
                                {
                                    if (i == 0)
                                    {
                                        sb.Append(AppsToIgnorePortfolioResult[i].ToString());
                                    }
                                    else
                                    {
                                        sb.Append("," + AppsToIgnorePortfolioResult[i].ToString());
                                    }
                                }
                            }

                            if (SnapsToIgnore.Count() > 0 && SnapsToIgnore != null)
                            {
                                SnapsToIgnore = SnapsToIgnore.Distinct().ToArray();
                                sb.Append(" Ignored Snapshots are: ");
                                for (int i = 0; i < SnapsToIgnore.Count(); i++)
                                {
                                    if (i == 0)
                                    {
                                        sb.Append(_ActiveConnection.Url + "/" + SnapsToIgnore[i].ToString());
                                    }
                                    else
                                    {
                                        sb.Append("," + _ActiveConnection.Url + "/" + SnapsToIgnore[i].ToString());
                                    }
                                }
                            }
                            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, sb.ToString() + "", null);

                            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Report generated", stopWatchStep.Elapsed);


                            //Show final message and unlock the screen   
                            stopWatchGlobal.Stop();
                            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string, TimeSpan>(base.MessageManager.OnReportGenerated), ReportFileName, stopWatchGlobal.Elapsed);
                            App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(base.MessageManager.SetBusyMode), false);
                        }

                    }
                    catch (System.Net.WebException webEx)
                    {
                        LogHelper.Instance.LogErrorFormat
                               ("Request URL '{0}' - Error execution :  {1}"
                               , ""
                               , webEx.Message
                               );

                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<double, string, TimeSpan>(base.MessageManager.OnStepDone), progressStep, "Error Generating Report - " + webEx.Message + " - Typically happens when Report Generator does not find REST API (in schema)", stopWatchStep.Elapsed);
                        stopWatchGlobal.Stop();
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(base.MessageManager.SetBusyMode), false);


                    }
                    catch (Exception ex)
                    {
                        App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<Exception>(WorkerThreadException), ex);
                    }
                } 
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateReport()
        {
            string tmpReportFile = String.Empty;
            string tmpReportFileFlexi = String.Empty;

            try
            {

                //Create temporary report
                string workDirectory = SettingsBLL.GetApplicationPath();
                tmpReportFile = PathUtil.CreateTempCopy(workDirectory, SelectedTemplateFile.FullName);
                if (tmpReportFile.Contains(".xlsx"))
                {
                    tmpReportFileFlexi = PathUtil.CreateTempCopyFlexi(workDirectory, SelectedTemplateFile.FullName);
                }
                //Build report
                ReportData reportData = new ReportData()
                {
                    FileName = tmpReportFile,
                    Application = SelectedApplication,
                    CurrentSnapshot = SelectedSnapshot,
                    PreviousSnapshot = PreviousSnapshot,
                    Parameter = Setting.ReportingParameter,
                    RuleExplorer = new RuleBLL(ActiveConnection),
                    SnapshotExplorer = new SnapshotBLL(ActiveConnection, SelectedSnapshot),
                    CurrencySymbol = "$"
                };


               
                using (IDocumentBuilder docBuilder = BuilderFactory.CreateBuilder(reportData, tmpReportFileFlexi))
                {
                    docBuilder.BuildDocument();
                }


                if (tmpReportFile.Contains(".xlsx"))
                {
                    tmpReportFile = tmpReportFileFlexi;
                }

                //Copy report file to the selected destination
                File.Copy(tmpReportFile, ReportFileName, true);
            }
            catch (Exception ex)
            {
                ReportFileName = String.Empty;

                throw (ex);
            }
            finally
            {
                if (!String.IsNullOrEmpty(tmpReportFile)) File.Delete(tmpReportFile);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void GenerateReportPortfolio(Application[] Applications, Snapshot[] ApplicationsSnapshots, string[] IgnoredApps, string[] IgnoredSnapshots )
        {
            string tmpReportFile = String.Empty;
            string tmpReportFileFlexi = String.Empty;

            try
            {

                //Create temporary report
                string workDirectory = SettingsBLL.GetApplicationPath();
                tmpReportFile = PathUtil.CreateTempCopy(workDirectory, SelectedTemplateFile.FullName);
                if (tmpReportFile.Contains(".xlsx"))
                {
                    tmpReportFileFlexi = PathUtil.CreateTempCopyFlexi(workDirectory, SelectedTemplateFile.FullName);
                }
                //Build report
                ReportData reportData = new ReportData()
                {
                    FileName = tmpReportFile,
                    Application = null,
                    CurrentSnapshot = null,
                    PreviousSnapshot = null,
                    Parameter = Setting.ReportingParameter,
                    RuleExplorer = new RuleBLL(ActiveConnection),
                    SnapshotExplorer = new SnapshotBLL(ActiveConnection, SelectedSnapshot),
                    CurrencySymbol = "$",
                    Applications = Applications,
                    Category = SelectedCategory,
                    Tag = SelectedTag,
                    snapshots = ApplicationsSnapshots,
                    IgnoresApplications = IgnoredApps,
                    IgnoresSnapshots = IgnoredSnapshots
                };



                using (IDocumentBuilder docBuilder = BuilderFactory.CreateBuilder(reportData, tmpReportFileFlexi))
                {
                    docBuilder.BuildDocument();
                }


                if (tmpReportFile.Contains(".xlsx"))
                {
                    tmpReportFile = tmpReportFileFlexi;
                }

                //Copy report file to the selected destination
                File.Copy(tmpReportFile, ReportFileName, true);
            }
            catch (Exception ex)
            {
                ReportFileName = String.Empty;

                throw (ex);
            }
            finally
            {
                if (!String.IsNullOrEmpty(tmpReportFile)) File.Delete(tmpReportFile);
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        public void InitializeFromWS()
        {
            //GetActive Connection           
            ActiveConnection = (Setting != null) ? Setting.GetActiveConnection() : null;

            //Get list of domains
            if (_ActiveConnection != null)
            {
                try
                {
                    using (CastDomainBLL castDomainBLL = new CastDomainBLL(ActiveConnection))
                    {
                        Applications = castDomainBLL.GetApplications();
                        Categories = castDomainBLL.GetCategories();
                        SelectedTab = 0;
                    }
                }
                catch(Exception ex)
                {
                    base.MessageManager.OnErrorOccured(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public void ActiveCurrentWebService(WSConnection connection)
        {
            StatesEnum state;
            Setting = SettingsBLL.AddConnection(connection, true, out state);

            base.MessageManager.OnServiceAdded(connection.Uri == null ? string.Empty : connection.Uri.ToString(), state);
        }
    }
}
