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
        Application[] _Application;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="application"></param>
        public PortfolioBLL(WSConnection connection, Application[] application)
            : base(connection)
        {

            _Application = application;
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
                if (!_Application.Any()) return ignoreApps;
                for (int i = 0; i < _Application.Count(); i++)
                {
                    try
                    {
                        _Application[i].Snapshots = castRepsitory.GetSnapshotsByApplication(_Application[i].Href);
                        _Application[i].Systems = castRepsitory.GetSystemsByApplication(_Application[i].Href);
                    }
                    catch (WebException webEx)
                    {
                        LogHelper.Instance.LogInfo(webEx.Message);
                        ignoreApps.Add(_Application[i].Name);
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

            string strBusinessCriterias = "business-criteria";

            using (var castRepsitory = GetRepository())
            {
                if (_Application.Any())
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        try
                        {
                            _Application[i].BusinessCriteriaResults = castRepsitory.GetResultsQualityIndicators(_Application[i].Href, strBusinessCriterias, "$all", string.Empty, string.Empty, string.Empty)
                                                                                     .ToList();
                        }
                        catch (WebException ex)
                        {
                            LogHelper.Instance.LogInfo(ex.Message);
                            ignoreApps.Add(_Application[i].Name);
                        }
                    }
                }
            }

            if (!_Application.Any()) return ignoreApps;
            {
                for (int i = 0; i < _Application.Count(); i++)
                {
                    try
                    {
                        if (_Application[i].Snapshots == null) continue;
                        foreach (var snapshot in _Application[i].Snapshots)
                        {
                            snapshot.BusinessCriteriaResults = _Application[i].BusinessCriteriaResults
                                .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                .Select(_ => _.ApplicationResults).FirstOrDefault();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                        ignoreApps.Add(_Application[i].Name);
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
                if (_Application.Any())
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        try
                        {
                            try
                            {
                                if (VersionUtil.IsAdgVersion82Compliant(_Application[i].AdgVersion))
                                {
                                    const string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                                    _Application[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application[i].Href, strSizingMeasures, "$all", string.Empty, string.Empty).ToList();
                                }
                                else
                                {
                                    const string strSizingMeasureOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                                    _Application[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application[i].Href, strSizingMeasureOld, "$all", string.Empty, string.Empty).ToList();
                                }

                            }
                            catch (WebException ex)
                            {
                                LogHelper.Instance.LogInfo(ex.Message);
                                const string strSizingMeasureOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                                _Application[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application[i].Href, strSizingMeasureOld, "$all", string.Empty, string.Empty).ToList();
                            }
                        }
                        catch (WebException ex)
                        {
                            LogHelper.Instance.LogInfo(ex.Message);
                            ignoreApps.Add(_Application[i].Name);
                        }
                    }
                }
            }

            if (!_Application.Any()) return ignoreApps;
            {
                for (int i = 0; i < _Application.Count(); i++)
                {
                    try
                    {
                        if (_Application[i].Snapshots == null) continue;
                        foreach (var snapshot in _Application[i].Snapshots)
                        {
                            snapshot.SizingMeasuresResults = _Application[i].SizingMeasuresResults
                                .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                .Select(_ => _.ApplicationResults).FirstOrDefault();
                        }
                    }
                    catch (WebException ex)
                    {
                        LogHelper.Instance.LogInfo(ex.Message);
                        ignoreApps.Add(_Application[i].Name);
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
                List<string> appsSetSnapshots = applicationBLL.SetSnapshots();
                List<string> appsSetQualityIndicators = applicationBLL.SetQualityIndicators();
                List<string> appsSetSizingMeasure = applicationBLL.SetSizingMeasure();

                string[] appsToIgnore = appsSetQualityIndicators.Concat(appsSetSnapshots).Concat(appsSetSizingMeasure).ToArray();
                return appsToIgnore;
                //Task taskSetSnapshots = new Task(() => applicationBLL.SetSnapshots());
                //taskSetSnapshots.Start();


                //Task taskQualityIndicators = new Task(() => applicationBLL.SetQualityIndicators());
                //taskQualityIndicators.Start();


                ////Build Quality Indicators
                //Task taskSizingMeasure = new Task(() => applicationBLL.SetSizingMeasure());
                //taskSizingMeasure.Start();

                //taskSetSnapshots.Wait();
                //taskQualityIndicators.Wait();
                //taskSizingMeasure.Wait();
            }

        }
    }
}
