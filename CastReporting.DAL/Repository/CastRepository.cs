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
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using CastReporting.Domain;
using CastReporting.Mediation;
using CastReporting.Mediation.Interfaces;
using CastReporting.Repositories.Interfaces;


namespace CastReporting.Repositories
{
    /// <summary>
    /// Cast reporting Context Class
    /// </summary>
    public class CastRepository : ICastRepsitory
    {
        #region CONSTANTS
       
        //wow7010/applications/12/results?quality-indicators=(business-criteria,technical-criteria,quality-rules,quality-distributions,quality-measures)&sizing-measures=(technical-size-measures,run-time-statistics,technical-debt-statistics,critical-violation-statistics,functional-weight-measures)&background-facts=(66061,66002,66004,66006,66001,66003,66005,66007)
        private const string _query_result_quality_indicators = "{0}/results?quality-indicators=({1})&snapshots=({2})&modules=({3}))&technologies=({4})&categories=({5})&select=evolutionSummary";
        private const string _query_result_sizing_measures = "{0}/results?sizing-measures=({1})&snapshots=({2})&technologies=({3})&modules=({4})";      
        private const string _query_configuration = "{0}/configuration/snapshots/{1}";
        private const string _query_action_plan = "{0}/action-plan/summary";
        private const string _query_action_plan2 = "{0}/actionPlan/summary";
        private const string _query_result_rules_violations = "{0}/results?quality-indicators={1}{2}&select=violationRatio&modules=($all)";
        private const string _query_result_quality_distribution_complexity = "{0}/results?quality-indicators=({1})&select=(categories)";
        private const string _query_rule_patterns = "{0}/rule-patterns/{1}";
        private const string _query_rules_details = "{0}/quality-indicators/{1}/snapshots/{2}/base-quality-indicators";
        private const string _query_transactions = "{0}/transactions/{1}?nbRows={2}";
        private const string _query_ifpug_functions = "{0}/ifpug-functions";
        private const string _query_components = "{0}/components/{1}?nbRows={2}";
        private const string _query_components_by_modules = "{0}/modules/{1}/snapshots/{2}/components/{3}?nbRows={4}";
        
        #endregion CONSTANTS

        #region ATTRIBUTES
        
        /// <summary>
        /// 
        /// </summary>
        private ICastProxy _Client;
        
        #endregion ATTRIBUTES

        #region PROPERTIES

        /// <summary>
        /// Get/Set the current connection.
        /// </summary>
        private string  _CurrentConnection;
        public string CurrentConnection
        {
            get
            {
                if (this._CurrentConnection == null) throw new TypeLoadException("Rest connection not set");
                return this._CurrentConnection;
            }           
        }
        #endregion PROPERTIES

        #region CONSTRUCTORS
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        public CastRepository(WSConnection connection)
        {
            this._Client = new CastProxy(connection.Login, connection.Password);
            this._CurrentConnection = connection.Url;
        }

        #endregion CONSTRUCTORS

        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            if (null != _Client)
            {
                _Client.Dispose();
            }
        }


        #region Databases

        /// <summary>
        /// Is Service Valid
        /// </summary>
        /// <param name="pMessage">Output message</param>
        /// <returns>True if OK</returns>
        bool ICastRepsitory.IsServiceValid()
        {
            try
            {
                var jsonString = this.CallWS<string>("/ping", RequestComplexity.Standard);
                              
            }
            catch
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<CastDomain> ICastRepsitory.GetDomains()
        {
            return this.ListSet<CastDomain>(string.Empty, string.Empty);           
        }
            
        #endregion Databases

        #region Applications

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Application> ICastRepsitory.GetApplicationsByDomain(string domainHRef)
        {
            return this.ListSet<Application>(domainHRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <returns></returns>
        Application ICastRepsitory.GetApplication(string hRef)
        {
            return this.Get<Application>(hRef);
        }

           
        #endregion Applications

        #region Snapshots
        IEnumerable<Component> ICastRepsitory.GetComponents(string snapshotHref, string businessCriteria, int count)
        {
            var requestUrl = string.Format(_query_components, snapshotHref, businessCriteria, count);

            return this.CallWS<IEnumerable<Component>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<Component> ICastRepsitory.GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        {
            var requestUrl = string.Format(_query_components_by_modules, domainId, moduleId, snapshotId, businessCriteria, count);

            return this.CallWS<IEnumerable<Component>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<Transaction> ICastRepsitory.GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            var requestUrl = string.Format(_query_transactions, snapshotHref, businessCriteria, count);

            return this.CallWS<IEnumerable<Transaction>>(requestUrl, RequestComplexity.Standard);
        }


        IEnumerable<IfpugFunction> ICastRepsitory.GetIfpugFunctions(string snapshotHref, int count)
        {
            var requestUrl = string.Format(_query_ifpug_functions, snapshotHref);

            return this.CallCsvWS<IfpugFunction>(requestUrl, RequestComplexity.Long, count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Snapshot> ICastRepsitory.GetSnapshotsByApplication(string applicationHRef)
        {
            return this.ListSet<Snapshot>(applicationHRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Domain.System> ICastRepsitory.GetSystemsByApplication(string applicationHRef)
        {
            return this.ListSet<Domain.System>(applicationHRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HRef"></param>
        /// <returns></returns>
        Snapshot ICastRepsitory.GetSnapshot(string hRef)
        {
            return this.Get<Snapshot>(hRef);
        }

        #endregion Snapshots

        #region Rules
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        RuleDescription ICastRepsitory.GetSpecificRule(string domain, string ruleId)
        {
            var requestUrl = string.Format(_query_rule_patterns, domain, ruleId);

            return this.CallWS<RuleDescription>(requestUrl, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="criticity"></param>
        /// <param name="businessCriteria"></param>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetRulesViolations(string snapshotHRef, string criticity, string businessCriteria)
        {
            if (!string.IsNullOrWhiteSpace(criticity))
            {
                criticity += ":";
            }

            var requestUrl = string.Format(_query_result_rules_violations, snapshotHRef, criticity, businessCriteria);
            
            return this.CallWS<IEnumerable<Result>>(requestUrl, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<RuleDetails> ICastRepsitory.GetRulesDetails(string domain, string businessCriteria, string snapshotId)
        {
            var requestUrl = string.Format(_query_rules_details, domain, businessCriteria, snapshotId);

            return this.CallWS<IEnumerable<RuleDetails>>(requestUrl, RequestComplexity.Standard);
        }
        #endregion

        #region ActionPlan
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<ActionPlan> ICastRepsitory.GetActionPlanBySnapshot(string snapshotHRef)
        {
            var requestUrl = string.Format(_query_action_plan, snapshotHRef);
            var requestUrl2 = string.Format(_query_action_plan2, snapshotHRef);
            
            try
            {
                return this.CallWS<IEnumerable<ActionPlan>>(requestUrl, RequestComplexity.Standard);
            }
            catch (WebException webEx)
            {
                // url for action plan has changed in API, and some old versions does not support the 2 format of the url
                return this.CallWS<IEnumerable<ActionPlan>>(requestUrl2, RequestComplexity.Standard);
            }
        }
        #endregion ActionPlan

        #region modules

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Module> ICastRepsitory.GetModules(string hRef)
        {
            return this.ListSet<Module>(hRef);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <returns></returns>
        Module ICastRepsitory.GetModule(string hRef)
        {
            return this.Get<Module>(hRef);
        }

        #endregion modules

        #region Results

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="qualityDistribution"></param>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetComplexityIndicators(string snapshotHRef, string qualityDistribution)
        {
            string relativeUrl = string.Format(_query_result_quality_distribution_complexity, snapshotHRef, qualityDistribution);

            return this.CallWS<IEnumerable<Result>>(relativeUrl, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetResultsQualityIndicators(string hRef, string qiParam, string snapshotsParam, string modulesParam, string technologiesParam, string categoriesParam)
        {
            string relativeURL = string.Format(_query_result_quality_indicators, hRef, qiParam, snapshotsParam, modulesParam, technologiesParam, categoriesParam);

            return this.CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetResultsSizingMeasures(string hRef, string param, string snapshotsParam, string technologiesParam, string moduleParam)
        {
            string relativeURL = string.Format(_query_result_sizing_measures, hRef, param, snapshotsParam, technologiesParam, moduleParam);

            return this.CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }

        #endregion Result

        #region Configuration

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainHRef"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<QIQualityRules> ICastRepsitory.GetConfQualityRulesBySnapshot(string domainHRef, Int64 snapshotId)
        {
            return this.ListConfiguration<QIQualityRules>(domainHRef, snapshotId, "quality-rules");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        QIQualityRules ICastRepsitory.GetConfQualityRules(string href)
        {
            return this.CallWS<QIQualityRules>(href, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<QIBusinessCriteria> ICastRepsitory.GetConfBusinessCriteriaBySnapshot(string domainHRef, Int64 snapshotId)
        {                    
            return this.ListConfiguration<QIBusinessCriteria>(domainHRef, snapshotId, "business-criteria");
        }

        QIBusinessCriteria ICastRepsitory.GetConfBusinessCriteria(string href)
        {
            return this.CallWS<QIBusinessCriteria>(href, RequestComplexity.Standard);
        }
        #endregion Configuration

        #region PRIVATE

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pEntity"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        private T CallWS<T>(string relativeURL, RequestComplexity pComplexity) where T : class
        {
             var requestUrl = _CurrentConnection.EndsWith("/") ? _CurrentConnection.Substring(0, _CurrentConnection.Length - 1) : _CurrentConnection;
            requestUrl += "/";
            requestUrl += relativeURL.StartsWith("/") ? relativeURL.Substring(1) : relativeURL;

            var jsonString = _Client.DownloadString(requestUrl, pComplexity); 

            var serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));

            return serializer.ReadObject(ms) as T;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeURL"></param>
        /// <param name="pComplexity"></param>
        /// <param name="count">Max items to load from CSV</param>
        /// <param name="PropNames">Mapping between CSV columns and T members (optional)</param>
        /// <returns></returns>
        private IEnumerable<T> CallCsvWS<T>(string relativeURL, RequestComplexity pComplexity, int count, params string[] PropNames) where T : new()
        {
            var requestUrl = _CurrentConnection.EndsWith("/") ? _CurrentConnection.Substring(0, _CurrentConnection.Length - 1) : _CurrentConnection;
            requestUrl += "/";
            requestUrl += relativeURL.StartsWith("/") ? relativeURL.Substring(1) : relativeURL;

            var csvString = _Client.DownloadCsvString(requestUrl, pComplexity);
       
            var serializer = new CsvSerializer<T>();
            return serializer.ReadObjects(csvString, count, PropNames);
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pRelativeUrl"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        private IEnumerable<T> ListSet<T>(string pRelativeUrl) where T : class
        {
            string typeName = typeof(T).Name.ToLower();
            string setName = typeName.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) ? typeName : typeName + "s";

            return this.ListSet<T>(pRelativeUrl, setName);
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pEntity"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        private IEnumerable<T> ListSet<T>(string pRelativeUrl, string pSetName) where T : class
        {           
             string url = string.Empty;

            if (!string.IsNullOrEmpty(pRelativeUrl)) url = pRelativeUrl; 
                
            if(!string.IsNullOrEmpty(pSetName)) url += "/" + pSetName;

            return CallWS<IEnumerable<T>>(url, RequestComplexity.Standard);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainHRef"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        private IEnumerable<T> ListConfiguration<T>(string domainHRef, Int64 snapshotId, string setName) where T : class
        {

            string relativeURL = string.Format(_query_configuration, domainHRef, snapshotId);

            return this.ListSet<T>(relativeURL, setName);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pUrl"></param>
        /// <param name="pComplexity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private T Get<T>(string id) where T : class
        {
             return CallWS<T>(id, RequestComplexity.Standard);                     
        }

       

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pException"></param>
        //private void ManageWebException(WebException pException)
        //{
        //    if (pException == null)
        //        return;

        //    #region WebException Management
        //    switch (pException.Status)
        //    {
        //        case WebExceptionStatus.Timeout:
        //            {
        //                throw new NotSupportedException(_errorMessage_requestTimeout, pException);
        //            }
        //        case WebExceptionStatus.NameResolutionFailure:
        //            {
        //                throw new NotSupportedException(_errorMessage_unknownServer, pException);
        //            }
        //        case WebExceptionStatus.ProtocolError:
        //            {
        //                HttpWebResponse res = pException.Response as HttpWebResponse;
        //                string message = _errorMessage_unknownError;
        //                if (null != res)
        //                {
        //                    message = string.Format(_errorMessage_genericError, res.StatusCode.GetHashCode(), res.StatusDescription);                            
        //                }
        //                throw new NotSupportedException(message, pException);
        //            }
        //        default:
        //            {
        //                HttpWebResponse res = pException.Response as HttpWebResponse;
        //                string message = _errorMessage_unknownError;
        //                if (null != res)
        //                {
        //                    message = string.Format(_errorMessage_genericError, res.StatusCode, res.StatusDescription);
        //                }
        //                throw new NotSupportedException(message, pException);
        //            }
        //    }
        //    #endregion WebException Management
        //}
        #endregion

      
    }
}
