using System;
using System.Linq;
using CastReporting.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

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
        /// <param name="currentSnapshot"></param>
        /// <param name="previousSnapshot"></param>
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
            List<string> IgnoreApps = new List<string>();
            using (var castRepsitory = GetRepository())
            {
                if (_Application.Count() > 0)
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        try
                        {
                            _Application[i].Snapshots = castRepsitory.GetSnapshotsByApplication(_Application[i].Href);
                            _Application[i].Systems = castRepsitory.GetSystemsByApplication(_Application[i].Href);
                        }
                        catch (WebException webEx)
                        {
                            IgnoreApps.Add(_Application[i].Name);
                            continue;
                        }
                    }
                }
            }
            return IgnoreApps;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SetQualityIndicators()
        {
            List<string> IgnoreApps = new List<string>();

            Int32[] businessCriterias = (Int32[])Enum.GetValues(typeof(Constants.BusinessCriteria));

            string strBusinessCriterias = string.Join(",", businessCriterias);

            using (var castRepsitory = GetRepository())
            {
                if (_Application.Count() > 0)
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
                            IgnoreApps.Add(_Application[i].Name);
                            continue;
                        }
                    }
                }
            }

            if (_Application.Count() > 0)
            {
                for (int i = 0; i < _Application.Count(); i++)
                {
                    try
                    {
                        if (_Application[i].Snapshots != null)
                        {
                            foreach (var snapshot in _Application[i].Snapshots)
                            {
                                snapshot.BusinessCriteriaResults = _Application[i].BusinessCriteriaResults
                                                                                    .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                                                                    .Select(_ => _.ApplicationResults).FirstOrDefault();
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        IgnoreApps.Add(_Application[i].Name);
                        continue;
                    }
                }
            }
            return IgnoreApps;
        }

     
        /// <summary>
        /// 
        /// </summary>
        public List<string> SetSizingMeasure()
        {
            List<string> IgnoreApps = new List<string>();
            // Int32[] sizingMeasures = (Int32[])Enum.GetValues(typeof(Constants.SizingInformations));
            // string strSizingMeasures = string.Join(",", sizingMeasures);

            // to get the results of all sizing measures in the snapshot, even is not in the list of known measures
            string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";

            using (var castRepsitory = GetRepository())
            {
                if (_Application.Count() > 0)
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        try
                        {
                            _Application[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application[i].Href, strSizingMeasures, "$all", string.Empty, string.Empty)
                                                                              .ToList();
                        }
                        catch (WebException ex)
                        {
                            IgnoreApps.Add(_Application[i].Name);
                            continue;
                        }
                    }
                }
            }

            if (_Application.Count() > 0)
            {
                for (int i = 0; i < _Application.Count(); i++)
                {
                    try
                    {
                        if (_Application[i].Snapshots != null)
                        {
                            foreach (var snapshot in _Application[i].Snapshots)
                            {
                                snapshot.SizingMeasuresResults = _Application[i].SizingMeasuresResults
                                                                                    .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                                                                    .Select(_ => _.ApplicationResults).FirstOrDefault();
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        IgnoreApps.Add(_Application[i].Name);
                        continue;
                    }
                }
            }
            return IgnoreApps;
        }



        /// <summary>
        /// 
        /// </summary>
        static public string[] BuildPortfolioResult(WSConnection connection, Application[] application)
        {
            //Build Quality Indicators
            using (PortfolioBLL applicationBLL = new PortfolioBLL(connection, application))
            {
                List<string> Apps_SetSnapshots = applicationBLL.SetSnapshots();
                List<string> Apps_SetQualityIndicators = applicationBLL.SetQualityIndicators();
                List<string> Apps_SetSizingMeasure = applicationBLL.SetSizingMeasure();

                string[] AppsToIgnore = Apps_SetQualityIndicators.Concat(Apps_SetSnapshots).Concat(Apps_SetSizingMeasure).ToArray();
                return AppsToIgnore;
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
