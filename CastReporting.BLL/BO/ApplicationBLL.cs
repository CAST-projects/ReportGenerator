/*
 *   Copyright (c) 2019 CAST
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

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CastReporting.Domain;
using System.Threading.Tasks;
using Cast.Util.Log;
using Cast.Util.Version;

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
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private Application _Application;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="application"></param>
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
                foreach(Snapshot snapshot in _Application.Snapshots)
                {
                    if (snapshot.AdgVersion == null && _Application.Version != null)
                        snapshot.AdgVersion = _Application.Version;
                }
                _Application.Systems = castRepsitory.GetSystemsByApplication(_Application.Href);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetQualityIndicators()
        {
            const string strBusinessCriteria = "business-criteria";

            using (var castRepsitory = GetRepository())
            {
                _Application.BusinessCriteriaResults = castRepsitory.GetResultsQualityIndicators(_Application.Href, strBusinessCriteria, "$all", string.Empty, string.Empty)?.ToList();
            }

            if (_Application.BusinessCriteriaResults == null) return;
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

            using (var castRepsitory = GetRepository())
            {
                try
                {
                    if (VersionUtil.IsAdgVersion82Compliant(_Application.Version))
                    {
                        const string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                        _Application.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application.Href, strSizingMeasures, "$all", string.Empty, string.Empty)?.ToList();
                    }
                    else
                    {
                        const string strSizingMeasuresOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                        _Application.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application.Href, strSizingMeasuresOld, "$all", string.Empty, string.Empty)?.ToList();
                    }
                }
                catch (System.Net.WebException ex)
                {
                    LogHelper.LogInfo(ex.Message);
                    const string strSizingMeasuresOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                    _Application.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Application.Href, strSizingMeasuresOld, "$all", string.Empty, string.Empty)?.ToList();
                }
                
            }

            if (_Application.SizingMeasuresResults == null) return;
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
        public void SetStandardTags()
        {

            using (var castRepsitory = GetRepository())
            {
                try
                {
                    if (VersionUtil.IsAdgVersion833Compliant(_Application.Version))
                    {
                        _Application.StandardTags = castRepsitory.GetQualityStandardsTagsDoc(_Application.Href);
                    }
                }
                catch (System.Net.WebException ex)
                {
                    LogHelper.LogInfo(ex.Message);
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public static void BuildApplicationResult(WSConnection connection, Application application)
        {
            //Build Quality Indicators
            using (ApplicationBLL applicationBLL = new ApplicationBLL(connection, application))
            {
                
                Task taskQualityIndicators = new Task(() => applicationBLL.SetQualityIndicators());
                taskQualityIndicators.Start();


                //Build Quality Indicators
                Task taskSizingMeasure = new Task(() => applicationBLL.SetSizingMeasure());
                taskSizingMeasure.Start();

                //Build Standard Tags doc
                Task taskStandardTags = new Task(() => applicationBLL.SetStandardTags());
                taskStandardTags.Start();


                taskQualityIndicators.Wait();
                taskSizingMeasure.Wait();
                taskStandardTags.Wait();
            }

        }
       
    }
}
