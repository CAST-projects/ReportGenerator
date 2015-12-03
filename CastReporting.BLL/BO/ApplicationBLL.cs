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
using System;
using System.Linq;
using CastReporting.Domain;
using System.Threading.Tasks;

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class ApplicationBLL : BaseBLL
    {
        /// <summary>
        /// 
        /// </summary>
        Application _Application;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSnapshot"></param>
        /// <param name="previousSnapshot"></param>
        public ApplicationBLL(WSConnection connection, Application application)
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
                _Application.Snapshots = castRepsitory.GetSnapshotsByApplication(_Application.Href);
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
                _Application.BusinessCriteriaResults = castRepsitory.GetResultsQualityIndicators(_Application.Href, strBusinessCriterias, "$all", string.Empty, string.Empty, string.Empty)
                                                                         .ToList();
            }

            foreach (var snapshot in _Application.Snapshots)
            {
                snapshot.BusinessCriteriaResults = _Application.BusinessCriteriaResults
                                                                    .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                                                    .Select(_ => _.ApplicationResults).FirstOrDefault();
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
                _Application.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application.Href, strSizingMeasures, "$all", string.Empty, string.Empty)
                                                                  .ToList();
            }

            foreach (var snapshot in _Application.Snapshots)
            {
                snapshot.SizingMeasuresResults = _Application.SizingMeasuresResults
                                                                    .Where(_ => _.Snapshot.Href.Equals(snapshot.Href))
                                                                    .Select(_ => _.ApplicationResults).FirstOrDefault();
            }
            
        }



        /// <summary>
        /// 
        /// </summary>
        static public void BuildApplicationResult(WSConnection connection, Application application)
        {
            //Build Quality Indicators
            using (ApplicationBLL applicationBLL = new ApplicationBLL(connection, application))
            {
                Task taskQualityIndicators = new Task(() => applicationBLL.SetQualityIndicators());
                taskQualityIndicators.Start();


                //Build Quality Indicators
                Task taskSizingMeasure = new Task(() => applicationBLL.SetSizingMeasure());
                taskSizingMeasure.Start();


                taskQualityIndicators.Wait();
                taskSizingMeasure.Wait();
            }

        }
       
    }
}
