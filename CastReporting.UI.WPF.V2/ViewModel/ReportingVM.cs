/*
 *   Copyright (c) 2015 CAST
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
                return _SelectedApplication != null &&
                       _SelectedSnapshot != null &&
                       _SelectedTemplateFile != null;
            }
        }


        /// <summary>
        /// 
        /// </summary>      
        public bool IsDataFilledIn
        {
            get
            {
                return _SelectedApplication != null &&
                       _SelectedSnapshot != null &&
                       _SelectedTemplateFile != null;
            }
        }
     
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
          
            //Load Tempalte
            LoadTemplatesCommand.Execute(null);
        }

        /// <summary>
        /// Implement Command that Load the templates list
        /// </summary>
        void ExecuteLoadTemplatesCommand(object parameter)
        {
            TemplateFiles = SettingsBLL.GetTemplateFileList();
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
            double progressStep = 100 / 4;
            Stopwatch stopWatchStep = new Stopwatch();
            Stopwatch stopWatchGlobal = new Stopwatch();

            try
            {
                stopWatchGlobal.Start();
                App.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action<bool>(base.MessageManager.SetBusyMode), true);

                //Set culture for the new thread
                if(!string.IsNullOrEmpty(Setting.ReportingParameter.CultureName))
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

        /// <summary>
        /// 
        /// </summary>
        private void GenerateReport()
        {
            string tmpReportFile = String.Empty;

            try
            {

                //Create temporary report
                string workDirectory = SettingsBLL.GetApplicationPath();
                tmpReportFile = PathUtil.CreateTempCopy(workDirectory, SelectedTemplateFile.FullName);

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


               
                using (IDocumentBuilder docBuilder = BuilderFactory.CreateBuilder(reportData))
                {
                    docBuilder.BuildDocument();
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
