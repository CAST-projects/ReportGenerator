using System.Linq;
using CastReporting.Domain;
using System.Collections.Generic;
using System.Net;
using Cast.Util.Version;
using Cast.Util.Log;

namespace CastReporting.BLL
{
    public class PortfolioBLL : BaseBLL
    {
         /// <summary>
        /// 
        /// </summary>
         protected Application[] Applications;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="applications"></param>
        public PortfolioBLL(WSConnection connection, Application[] applications)
            : base(connection)
        {

            Applications = applications;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> SetSnapshots()
        {
            List<string> ignoreApps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (!Applications.Any()) return ignoreApps;
                foreach (Application app in Applications)
                {
                    try
                    {
                        app.Snapshots = castRepsitory.GetSnapshotsByApplication(app.Href);
                        app.Systems = castRepsitory.GetSystemsByApplication(app.Href);
                    }
                    catch (WebException webEx)
                    {
                        LogHelper.LogInfo(webEx.Message);
                        ignoreApps.Add(app.Name);
                    }
                }
            }
            return ignoreApps;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SetQualityIndicators()
        {
            List<string> ignoreApps = new List<string>();

            const string strBusinessCriterias = "business-criteria";

            using (var castRepsitory = GetRepository())
            {
                if (Applications.Any())
                {
                    foreach (Application app in Applications)
                    {
                        try
                        {
                            app.BusinessCriteriaResults = castRepsitory.GetResultsQualityIndicators(app.Href, strBusinessCriterias, "$all", string.Empty, string.Empty)?.ToList();
                        }
                        catch (WebException ex)
                        {
                            LogHelper.LogInfo(ex.Message);
                            ignoreApps.Add(app.Name);
                        }
                    }
                }
            }

            if (!Applications.Any()) return ignoreApps;
            {
                foreach (Application app in Applications)
                {
                    try
                    {
                        if (app.Snapshots == null || app.BusinessCriteriaResults == null) continue;
                        foreach (var snapshot in app.Snapshots)
                        {
                            snapshot.BusinessCriteriaResults = app.BusinessCriteriaResults
                                .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                .Select(_ => _.ApplicationResults).FirstOrDefault();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        ignoreApps.Add(app.Name);
                    }
                }
            }
            return ignoreApps;
        }

     
        /// <summary>
        /// 
        /// </summary>
        public List<string> SetSizingMeasure()
        {
            List<string> ignoreApps = new List<string>();

            using (var castRepsitory = GetRepository())
            {
                if (Applications.Any())
                {
                    foreach (Application app in Applications)
                    {
                        try
                        {
                            try
                            {
                                if (VersionUtil.IsAdgVersion82Compliant(app.AdgVersion))
                                {
                                    const string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                                    app.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(app.Href, strSizingMeasures, "$all", string.Empty, string.Empty)?.ToList();
                                }
                                else
                                {
                                    const string strSizingMeasureOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                                    app.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(app.Href, strSizingMeasureOld, "$all", string.Empty, string.Empty)?.ToList();
                                }

                            }
                            catch (WebException ex)
                            {
                                LogHelper.LogInfo(ex.Message);
                                const string strSizingMeasureOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                                app.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(app.Href, strSizingMeasureOld, "$all", string.Empty, string.Empty)?.ToList();
                            }
                        }
                        catch (WebException ex)
                        {
                            LogHelper.LogInfo(ex.Message);
                            ignoreApps.Add(app.Name);
                        }
                    }
                }
            }

            if (!Applications.Any()) return ignoreApps;
            {
                foreach (Application app in Applications)
                {
                    try
                    {
                        if (app.Snapshots == null || app.SizingMeasuresResults == null) continue;
                        foreach (var snapshot in app.Snapshots)
                        {
                            snapshot.SizingMeasuresResults = app.SizingMeasuresResults
                                .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                .Select(_ => _.ApplicationResults).FirstOrDefault();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.LogInfo(ex.Message);
                        ignoreApps.Add(app.Name);
                    }
                }
            }
            return ignoreApps;
        }



        /// <summary>
        /// 
        /// </summary>
        public static string[] BuildPortfolioResult(WSConnection connection, Application[] application)
        {
            //Build Quality Indicators
            using (PortfolioBLL applicationBLL = new PortfolioBLL(connection, application))
            {
                List <string> appsSetSnapshots = applicationBLL.SetSnapshots();
                List<string> appsSetQualityIndicators = applicationBLL.SetQualityIndicators();
                List<string> appsSetSizingMeasure = applicationBLL.SetSizingMeasure();

                string[] appsToIgnore = appsSetQualityIndicators.Concat(appsSetSnapshots).Concat(appsSetSizingMeasure).ToArray();
                return appsToIgnore;
            }

        }
    }
}
