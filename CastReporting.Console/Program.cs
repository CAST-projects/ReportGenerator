using Cast.Util;
using Cast.Util.Log;
using CastReporting.BLL;
using CastReporting.Console.Argument;
using CastReporting.Domain;
using CastReporting.Reporting.Builder;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace CastReporting.Console
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        { 
            string showhelp;
            
            LogHelper.SetPathLog(SettingsBLL.GetApplicationPath());
            
            SetCulture();
           
            LogHelper.Instance.LogInfo("Application started.");

            Environment.ExitCode = DoWork(args, out showhelp);

            if (!string.IsNullOrEmpty(showhelp)) System.Console.WriteLine(showhelp);

            // Uncomment if you want to see the console during debugging
            // System.Console.ReadLine();
        }


        /// <summary>
        /// 
        /// </summary>
        private static void SetCulture()
        {
            var settings = SettingsBLL.GetSetting();

            if (!String.IsNullOrEmpty(settings.ReportingParameter.CultureName))
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(settings.ReportingParameter.CultureName);

                if (cultureInfo != null)
                {
                    Thread.CurrentThread.CurrentCulture = cultureInfo;
                    Thread.CurrentThread.CurrentUICulture = cultureInfo;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="help"></param>
        /// <returns></returns>
        private static Int32 DoWork(string[] args, out string help)
        {
            LogHelper.Instance.LogInfo("Read arguments.");
            var arguments = ReadArguments(args, out help);

            if (!string.IsNullOrEmpty(help))
            {             
                return -1;
            }           

            string pathFile = GenerateReport(arguments, out help);

            if (!string.IsNullOrEmpty(help))
            {
                LogHelper.Instance.LogError(help);
                return -1;
            }
            else
            {
                LogHelper.Instance.LogInfo(string.Format("Report successfully generated in {0}", pathFile));
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="help"></param>
        /// <returns></returns>
        private static string GenerateReport(XmlCastReport arguments, out string help)
        {
            string tmpReportFile = string.Empty;

            try
            {
                help = string.Empty;

                //Get RG settings
                var settings = SettingsBLL.GetSetting();
                LogHelper.Instance.LogInfo("RG settings have been read successfully");

                //Initialize temporary directory
                string workDirectory = SettingsBLL.GetApplicationPath();
                tmpReportFile = PathUtil.CreateTempCopy(workDirectory, Path.Combine(settings.ReportingParameter.TemplatePath, arguments.Template.Name));

                //Initialize Web services

                var connection = new WSConnection(arguments.Webservice.Name, arguments.Username.Name, arguments.Password.Name, string.Empty);
                using (CommonBLL commonBLL = new CommonBLL(connection))
                {
                    if (!commonBLL.CheckService())
                    {
                        help = string.Format("Webservice can't be access or is bad formatted. Url:{0} Username:{1} Password:{2}", arguments.Webservice.Name, arguments.Username.Name, arguments.Password.Name);
                        return string.Empty;
                    }
                }
                LogHelper.Instance.LogInfo("Web services Initialized successfully");


                //Initialize Application
                Application application = GetApplication(arguments.Application.Name, connection);
                if (application == null)
                {
                    help = string.Format("Application {0} can't be found.", arguments.Application.Name);
                    return string.Empty;
                }
                LogHelper.Instance.LogInfo(string.Format("Application {0} Initialized successfully", arguments.Application.Name));

                //Initialize snapshots             
                SetSnapshots(connection, application);
                if (application.Snapshots == null)
                {
                    help = "There is no snapshots for this application.";
                    return string.Empty;
                }
                LogHelper.Instance.LogInfo(string.Format("List of Snapshots Initialized successfully", arguments.Application.Name));

                //Build Application results 
                ApplicationBLL.BuildApplicationResult(connection, application);
                LogHelper.Instance.LogInfo(string.Format("Application results built successfully", arguments.Application.Name));


                //Set current snapshot
                Snapshot currentSnapshot = GetSnapshotOrDefault(arguments.Snapshot.Current, application.Snapshots, 0);
                if (currentSnapshot == null)
                {
                    help = string.Format("Current snapshot {0} can't be found", arguments.Snapshot.Current.Name);
                    return string.Empty;
                }
                LogHelper.Instance.LogInfo(string.Format("Current snapshot {0} initialized successfully", currentSnapshot.Name));

                //Build current snapshot results 
                SnapshotBLL.BuildSnapshotResult(connection, currentSnapshot, true);
                LogHelper.Instance.LogInfo(string.Format("Result of current snapshot {0} built successfully", currentSnapshot.Name));

                //Set previous snapshot
                Snapshot prevSnapshot = GetSnapshotOrDefault(arguments.Snapshot.Previous, application.Snapshots, 1);
                if (arguments.Snapshot.Previous != null && !string.IsNullOrEmpty(arguments.Snapshot.Previous.Name) && prevSnapshot == null)
                {
                    help = string.Format("Previous snapshot {0} can't be found", arguments.Snapshot.Previous.Name);
                    return string.Empty;
                }
                if (prevSnapshot != null) LogHelper.Instance.LogInfo(string.Format("Previous snapshot {0} Initialized successfully", prevSnapshot.Name));

                //Build previous snapshot results 
                if (prevSnapshot != null)
                {
                    SnapshotBLL.BuildSnapshotResult(connection, prevSnapshot, false);
                    LogHelper.Instance.LogInfo(string.Format("Result of previous snapshot {0}  built successfully", prevSnapshot.Name));
                }


                //Build report              
                ReportData reportData = new ReportData()
                {
                    FileName = tmpReportFile,
                    Application = application,
                    CurrentSnapshot = currentSnapshot,
                    PreviousSnapshot = prevSnapshot,
                    Parameter = settings.ReportingParameter,
                    RuleExplorer = new RuleBLL(connection),
                    SnapshotExplorer = new SnapshotBLL(connection, currentSnapshot),
                    CurrencySymbol = "$"
                };

                using (IDocumentBuilder docBuilder = BuilderFactory.CreateBuilder(reportData))
                {
                    docBuilder.BuildDocument();
                }
                LogHelper.Instance.LogInfo("Report generated successfully");

                //Set filte report              
                SetFileName(arguments);

                string reportPath;
                if (string.IsNullOrEmpty(settings.ReportingParameter.GeneratedFilePath))
                    reportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), arguments.File.Name);
                else
                    reportPath = Path.Combine(settings.ReportingParameter.GeneratedFilePath, arguments.File.Name);


                File.Copy(tmpReportFile, reportPath, true);
                LogHelper.Instance.LogInfo("Report moved to generation directory successfully");

                return reportPath;
            }
            catch (Exception ex)
            {
                help = string.Format("An exception occured : {0}", ex);
                return string.Empty;
            }
            finally
            {
                if (!string.IsNullOrEmpty(tmpReportFile)) File.Delete(tmpReportFile);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        private static void SetFileName(XmlCastReport arguments)
        {
            if (arguments.File == null)
            {
                arguments.File = new XmlTagName() { Name = null };
            }
            if (string.IsNullOrEmpty(arguments.File.Name))
            {
                arguments.File.Name = string.Format
                ("{0}_{1}{2}"
                , Path.GetFileNameWithoutExtension(arguments.Template.Name)
                , DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")
                , Path.GetExtension(arguments.Template.Name)
                );

            }
        }


       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSnapshotName"></param>
        /// <param name="snapshosts"></param>
        /// <returns></returns>
        private static Snapshot GetSnapshotOrDefault(XmlTagName currentSnapshotName, IEnumerable<Snapshot> snapshosts, Int32 indexDefault)
        {
            Snapshot currentSnapshot=null;

            if (currentSnapshotName != null && !string.IsNullOrEmpty(currentSnapshotName.Name))
            {
                currentSnapshot = snapshosts.Where(_ => string.Format("{0} - {1}", _.Name, _.Annotation.Version) == currentSnapshotName.Name).FirstOrDefault();
            }
            else
            {
                currentSnapshot = snapshosts.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).ElementAtOrDefault(indexDefault);
            }
            return currentSnapshot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        private static void SetSnapshots(WSConnection connection, Application application)
        {          
            using (ApplicationBLL applicationBLL = new ApplicationBLL(connection, application))
            {
                applicationBLL.SetSnapshots();                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static Application GetApplication(string application, WSConnection connection)
        {
            List<Application> applications;

            using (CastDomainBLL castDomainBLL = new CastDomainBLL(connection))
            {
                applications = castDomainBLL.GetApplications();
            }

            return applications.Where(_ => _.Name == application).FirstOrDefault();
        }


        #region Argument Management
        /// <summary>
        /// Read arguments from Main
        /// </summary>
        /// <param name="pArg"></param>
        /// <param name="pShowHelp"></param>
        /// <returns></returns>
        public static XmlCastReport ReadArguments(string[] pArg, out string pShowHelp)
        {
            pShowHelp = null;
            XmlCastReport argument = null;

            // Get Arguments File Name
            using (CRArgumentReader reader = new CRArgumentReader())
            {
                bool show;
                argument = reader.Load(pArg, out show);
                if (show)
                    pShowHelp = reader.Help;
            }

            return argument;
        }
        #endregion
    }
}
