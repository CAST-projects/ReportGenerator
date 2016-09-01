using System;
using System.Linq;
using CastReporting.Domain;
using System.Threading.Tasks;

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
        public void SetSnapshots()
        {
            using (var castRepsitory = GetRepository())
            {
                if (_Application.Count() > 0)
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        _Application[i].Snapshots = castRepsitory.GetSnapshotsByApplication(_Application[i].Href);
                        _Application[i].Systems = castRepsitory.GetSystemsByApplication(_Application[i].Href);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetQualityIndicators()
        {
            Int32[] businessCriterias = (Int32[])Enum.GetValues(typeof(Constants.BusinessCriteria));

            string strBusinessCriterias = string.Join(",", businessCriterias);

            using (var castRepsitory = GetRepository())
            {
                if (_Application.Count() > 0)
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        _Application[i].BusinessCriteriaResults = castRepsitory.GetResultsQualityIndicators(_Application[i].Href, strBusinessCriterias, "$all", string.Empty, string.Empty, string.Empty)
                                                                                 .ToList();
                    }
                }
            }

            if (_Application.Count() > 0)
            {
                for (int i = 0; i < _Application.Count(); i++)
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
            }
        }

     
        /// <summary>
        /// 
        /// </summary>
        public void SetSizingMeasure()
        {
            Int32[] sizingMeasures = (Int32[])Enum.GetValues(typeof(Constants.SizingInformations));
            string strSizingMeasures = string.Join(",", sizingMeasures);

            using (var castRepsitory = GetRepository())
            {
                if (_Application.Count() > 0)
                {
                    for (int i = 0; i < _Application.Count(); i++)
                    {
                        _Application[i].SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application[i].Href, strSizingMeasures, "$all", string.Empty, string.Empty)
                                                                          .ToList();
                    }
                }
            }

            if (_Application.Count() > 0)
            {
                for (int i = 0; i < _Application.Count(); i++)
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
            }
        }



        /// <summary>
        /// 
        /// </summary>
        static public void BuildPortfolioResult(WSConnection connection, Application[] application)
        {
            //Build Quality Indicators
            using (PortfolioBLL applicationBLL = new PortfolioBLL(connection, application))
            {
                Task taskSetSnapshots = new Task(() => applicationBLL.SetSnapshots());
                taskSetSnapshots.Start();


                Task taskQualityIndicators = new Task(() => applicationBLL.SetQualityIndicators());
                taskQualityIndicators.Start();


                //Build Quality Indicators
                Task taskSizingMeasure = new Task(() => applicationBLL.SetSizingMeasure());
                taskSizingMeasure.Start();

                taskSetSnapshots.Wait();
                taskQualityIndicators.Wait();
                taskSizingMeasure.Wait();
            }

        }
    }
}
