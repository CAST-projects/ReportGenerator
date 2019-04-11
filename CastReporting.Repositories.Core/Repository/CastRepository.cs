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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Cast.Util.Log;
using CastReporting.Domain;
using CastReporting.Domain.Core.DataObject;
using CastReporting.Mediation;
using CastReporting.Mediation.Interfaces;
using CastReporting.Repositories.Interfaces;
// ReSharper disable InconsistentNaming


namespace CastReporting.Repositories
{
    /// <summary>
    /// Cast reporting Context Class
    /// </summary>
    public class CastRepository : ICastRepsitory
    {
        #region CONSTANTS
        
        // Sometimes modules, technologies, snapshots, categories are null and the rest api 8.2 does not support it anymore for security reasons
        private const string _query_result_quality_indicators = "{0}/results?quality-indicators=({1})&select=(evolutionSummary,violationRatio)";
        private const string _query_result_sizing_measures = "{0}/results?sizing-measures=({1})";
        private const string _query_result_background_facts = "{0}/results?background-facts=({1})";
        private const string _query_configuration = "{0}/configuration/snapshots/{1}";
        private const string _query_action_plan = "{0}/action-plan/summary";
        private const string _query_action_plan2 = "{0}/actionPlan/summary";
        private const string _query_result_rules_violations = "{0}/results?quality-indicators={1}{2}&select=violationRatio&modules=($all)";
        private const string _query_result_quality_distribution_complexity = "{0}/results?quality-indicators=({1})&select=(categories)";
        private const string _query_rule_patterns = "{0}/rule-patterns/{1}";
        private const string _query_rules_details = "{0}/quality-indicators/{1}/snapshots/{2}/base-quality-indicators";
        private const string _query_grade_contributors = "{0}/quality-indicators/{1}/snapshots/{2}";
        private const string _query_transactions = "{0}/transactions/{1}?nbRows={2}";
        private const string _query_ifpug_functions = "{0}/ifpug-functions";
        private const string _query_ifpug_functions_evolutions = "{0}/ifpug-functions-evolution";
        private const string _query_omg_functions_evolutions = "{0}/omg-functions-functional-evolution";
        private const string _query_metric_top_artefact = "{0}/violations?rule-pattern={1}";
        private const string _query_components = "{0}/components/{1}?nbRows={2}";
        private const string _query_components_with_properties = "{0}/components/{1}?properties=({2},{3})&order=({4})&startRow=1&nbRows={5}";
        private const string _query_components_by_modules = "{0}/modules/{1}/snapshots/{2}/components/{3}?nbRows={4}";
        private const string _query_common_categories = "{0}/AAD/common-categories";
        private const string _query_tags = "{0}/AAD/tags";
        private const string _query_violations_list_by_rule_bcid = "{0}/violations?rule-pattern={1}&business-criterion={2}&startRow=1&nbRows={3}&technologies={4}";
        private const string _query_action_plan_issues = "{0}/action-plan/issues?nbRows={1}";
        private const string _query_result_quality_standards_rules = "{0}/results?quality-indicators=(c:{1})";
        private const string _query_findings = "{0}/components/{1}/snapshots/{2}/findings/{3}";
        private const string _query_component_source_code = "{0}/components/{1}/snapshots/{2}/source-codes";
        private const string _query_file_content = "{0}/local-sites/{1}/file-contents/{2}?start-line={3}&end-line={4}";
        private const string _query_component_type = "{0}/components/{1}/snapshots/{2}";
        private const string _query_quality_standards_evolution = "{0}/results?quality-standards=(c:{1})&select=(evolutionSummary)";
        private const string _query_quality_standards_information = "{0}/quality-standards";
        private const string _query_removed_violations_by_bcid = "{0}/removed-violations?rule-pattern=(cc:{1},nc:{1})&nbRows={2}";
        private const string _query_delta_components = "{0}/components/65005?snapshot-ids=({1},{2})&status={3}";

        #endregion CONSTANTS

        #region ATTRIBUTES

        /// <summary>
        /// 
        /// </summary>
        protected ICastProxy _Client;
        
        #endregion ATTRIBUTES

        #region PROPERTIES

        /// <summary>
        /// Get/Set the current connection.
        /// </summary>
        protected string  _CurrentConnection;
        public string CurrentConnection
        {
            get
            {
                if (_CurrentConnection == null) throw new TypeLoadException("Rest connection not set");
                return _CurrentConnection;
            }           
        }
        #endregion PROPERTIES

        #region CONSTRUCTORS

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="client"></param>
        public CastRepository(WSConnection connection, ICastProxy client)
        {
            _Client = new CastProxy(connection.Login, connection.Password, client?.GetCookieContainer());
            
            _CurrentConnection = connection.Url;
        }

        public ICastProxy GetClient()
        {
            return _Client;
        }

        #endregion CONSTRUCTORS

        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            _Client?.Dispose();
        }


        #region Databases

        /// <summary>
        /// Is Service Valid
        /// </summary>
        /// <returns>True if OK</returns>
        bool ICastRepsitory.IsServiceValid()
        {
            var requestUrl = _CurrentConnection.EndsWith("/") ? _CurrentConnection.Substring(0, _CurrentConnection.Length - 1) : _CurrentConnection;
            requestUrl += "/ping";

            try
            {
                var jsonString = _Client.DownloadString(requestUrl, RequestComplexity.Standard);

                var serializer = new DataContractJsonSerializer(typeof(string));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
                try
                {
                    serializer.ReadObject(ms);
                }
                finally
                {
                    ms.Close();
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        string ICastRepsitory.GetServerVersion()
        {
            Server res = CallWS<Server>("server", RequestComplexity.Standard);
            return res?.Version;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<CastDomain> ICastRepsitory.GetDomains()
        {
            return ListSet<CastDomain>(string.Empty, string.Empty);           
        }
            
        #endregion Databases

        #region Applications

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Application> ICastRepsitory.GetApplicationsByDomain(string domainHRef)
        {
            return ListSet<Application>(domainHRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <returns></returns>
        Application ICastRepsitory.GetApplication(string hRef)
        {
            return Get<Application>(hRef);
        }


        #endregion Applications

        #region Snapshots
        List<string> ICastRepsitory.GetFileContent(string domainHRef, string siteId, string fileId, int startLine, int endLine)
        {
            var requestUrl = string.Format(_query_file_content, domainHRef, siteId, fileId, startLine, endLine);

            return CallStringWS(requestUrl, RequestComplexity.Standard).Split('\n').ToList();
        }

        AssociatedValue ICastRepsitory.GetAssociatedValue(string domainHRef, string snapshotId, string objectId, string metricId)
        {
            var requestUrl = string.Format(_query_findings, domainHRef, objectId, snapshotId, metricId);

            return CallWS<AssociatedValue>(requestUrl, RequestComplexity.Standard);

        }

        AssociatedValuePath ICastRepsitory.GetAssociatedValuePath(string domainHRef, string snapshotId, string objectId, string metricId)
        {
            var requestUrl = string.Format(_query_findings, domainHRef, objectId, snapshotId, metricId);

            return CallWS<AssociatedValuePath>(requestUrl, RequestComplexity.Standard);
        }

        AssociatedValueGroup ICastRepsitory.GetAssociatedValueGroup(string domainHRef, string snapshotId, string objectId, string metricId)
        {
            var requestUrl = string.Format(_query_findings, domainHRef, objectId, snapshotId, metricId);

            return CallWS<AssociatedValueGroup>(requestUrl, RequestComplexity.Standard);
        }

        AssociatedValueObject ICastRepsitory.GetAssociatedValueObject(string domainHRef, string snapshotId, string objectId, string metricId)
        {
            var requestUrl = string.Format(_query_findings, domainHRef, objectId, snapshotId, metricId);

            return CallWS<AssociatedValueObject>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<CodeFragment> ICastRepsitory.GetSourceCode(string domainHRef, string snapshotId, string objectId)
        {
            var requestUrl = string.Format(_query_component_source_code, domainHRef, objectId, snapshotId);

            return CallWS<IEnumerable<CodeFragment>>(requestUrl, RequestComplexity.Standard);
        }

        TypedComponent ICastRepsitory.GetTypedComponent(string domainHRef, string componentId, string snapshotId)
        {
            var requestUrl = string.Format(_query_component_type, domainHRef, componentId, snapshotId);

            return Get<TypedComponent>(requestUrl);
        }

        IEnumerable<Component> ICastRepsitory.GetComponents(string snapshotHref, string businessCriteria, int count)
        {
            var requestUrl = string.Format(_query_components, snapshotHref, businessCriteria, count);

            return CallWS<IEnumerable<Component>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<ComponentWithProperties> ICastRepsitory.GetComponentsWithProperties(string snapshothref, int bcId, string prop1 , string prop2, string order1, string order2, int count)
        {
            // rest api url : D/applications/A/snapshots/S/components/60017?properties=(cyclomaticComplexity,fanOut)&order=(desc(cyclomaticComplexity),desc(fanOut))&startRow=1&nbRows=50
            // _query_components_with_properties = {0}/components/{1}?properties=({2},{3})&order=({4})&startRow=1&nbRows={5}
            string order = order1.ToLower() + "(" + prop1 + ")," + order2.ToLower() + "(" + prop2 + ")"; 
            var requestUrl = string.Format(_query_components_with_properties, snapshothref, bcId, prop1, prop2, order, count);

            return CallWS<IEnumerable<ComponentWithProperties>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<Component> ICastRepsitory.GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        {
            var requestUrl = string.Format(_query_components_by_modules, domainId, moduleId, snapshotId, businessCriteria, count);
            
            return CallWS<IEnumerable<Component>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<Transaction> ICastRepsitory.GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            var requestUrl = string.Format(_query_transactions, snapshotHref, businessCriteria, count);

            return CallWS<IEnumerable<Transaction>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<CommonCategories> ICastRepsitory.GetCommonCategories()
        {
            var requestUrl = string.Format(_query_common_categories, "");

            return CallWS<IEnumerable<CommonCategories>>(requestUrl, RequestComplexity.Standard);
        }

        string ICastRepsitory.GetCommonCategoriesJson()
        {
            var requestUrl = string.Format(_query_common_categories, "");

            return CallWSJsonOnly(requestUrl, RequestComplexity.Standard); 
        }

        string ICastRepsitory.GetCommonTagsJson()
        {
            var requestUrl = string.Format(_query_tags, "");

            return CallWSJsonOnly(requestUrl, RequestComplexity.Standard);

        }

        IEnumerable<IfpugFunction> ICastRepsitory.GetIfpugFunctions(string snapshotHref, int count)
        {
            var requestUrl = string.Format(_query_ifpug_functions, snapshotHref);

            return CallCsvWS<IfpugFunction>(requestUrl, RequestComplexity.Long, count);
        }

        IEnumerable<IfpugFunction> ICastRepsitory.GetIfpugFunctionsEvolutions(string snapshotHref, int count)
        {
            var requestUrl = string.Format(_query_ifpug_functions_evolutions, snapshotHref);

            return CallCsvWS<IfpugFunction>(requestUrl, RequestComplexity.Long, count);
        }

        IEnumerable<OmgFunction> ICastRepsitory.GetOmgFunctionsEvolutions(string snapshotHref, int count)
        {
            var requestUrl = string.Format(_query_omg_functions_evolutions, snapshotHref);

            return CallCsvWS<OmgFunction>(requestUrl, RequestComplexity.Long, count);
        }

        IEnumerable<OmgFunctionTechnical> ICastRepsitory.GetOmgFunctionsTechnical(string snapshotHref, int count)
        {
            var requestUrl = string.Format(_query_omg_functions_evolutions + "?sheet=technical", snapshotHref);

            return CallCsvWS<OmgFunctionTechnical>(requestUrl, RequestComplexity.Long, count);
        }

        IEnumerable<MetricTopArtifact> ICastRepsitory.GetMetricTopArtefact(string snapshotHref, string RuleId, int count)
        {
            var requestUrl = string.Format(_query_metric_top_artefact, snapshotHref, RuleId);

            return CallCsvWS<MetricTopArtifact>(requestUrl, RequestComplexity.Long, count);
        }

        IEnumerable<Violation> ICastRepsitory.GetViolationsListIDbyBC(string snapshotHref, string RuleId, string bcId, int count, string technos)
        {
            var requestUrl = (count != -1) ? string.Format(_query_violations_list_by_rule_bcid, snapshotHref, RuleId, bcId, count,technos)
                    : string.Format(_query_violations_list_by_rule_bcid, snapshotHref, RuleId, bcId, "$all", technos);

            return CallWS<IEnumerable<Violation>>(requestUrl, RequestComplexity.Long);
        }

        IEnumerable<Violation> ICastRepsitory.GetViolationsInActionPlan(string snapshotHref, int count)
        {
            var requestUrl = (count != -1) ? string.Format(_query_action_plan_issues, snapshotHref, count)
                : string.Format(_query_action_plan_issues, snapshotHref, "$all") ;

            return CallWS<IEnumerable<Violation>>(requestUrl, RequestComplexity.Long);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationHRef"></param>
        /// <returns></returns>
        IEnumerable<Snapshot> ICastRepsitory.GetSnapshotsByApplication(string applicationHRef)
        {
            return ListSet<Snapshot>(applicationHRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationHRef"></param>
        /// <returns></returns>
        IEnumerable<Domain.System> ICastRepsitory.GetSystemsByApplication(string applicationHRef)
        {
            return ListSet<Domain.System>(applicationHRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <returns></returns>
        Snapshot ICastRepsitory.GetSnapshot(string hRef)
        {
            return Get<Snapshot>(hRef);
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

            return CallWS<RuleDescription>(requestUrl, RequestComplexity.Standard);
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
            
            return CallWS<IEnumerable<Result>>(requestUrl, RequestComplexity.Standard);
        }

        public IEnumerable<Violation> GetRemovedViolations(string snapshotHRef, string businessCriteria, int count)
        {
            var requestUrl = (count != -1) ? string.Format(_query_removed_violations_by_bcid, snapshotHRef, businessCriteria, count)
                : string.Format(_query_removed_violations_by_bcid, snapshotHRef, businessCriteria, "$all");

            return CallWS<IEnumerable<Violation>>(requestUrl, RequestComplexity.Long);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<RuleDetails> ICastRepsitory.GetRulesDetails(string domain, int businessCriteria, long snapshotId)
        {
            var requestUrl = string.Format(_query_rules_details, domain, businessCriteria, snapshotId);

            return CallWS<IEnumerable<RuleDetails>>(requestUrl, RequestComplexity.Standard);
        }

        IEnumerable<Contributor> ICastRepsitory.GetRulesForTechnicalCriteria(string domain, string technicalCriteria, long snapshotId)
        {
            var requestUrl = string.Format(_query_grade_contributors, domain, technicalCriteria, snapshotId);

            return Get<QITechnicalCriteria>(requestUrl).Contributors;
        }

        #endregion

        #region ActionPlan

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <returns></returns>
        IEnumerable<ActionPlan> ICastRepsitory.GetActionPlanBySnapshot(string snapshotHRef)
        {
            var requestUrl = string.Format(_query_action_plan, snapshotHRef);
            var requestUrl2 = string.Format(_query_action_plan2, snapshotHRef);
            
            try
            {
                return CallWS<IEnumerable<ActionPlan>>(requestUrl, RequestComplexity.Standard);
            }
            catch (WebException webEx)
            {
                LogHelper.Instance.LogInfo(webEx.Message);
                // url for action plan has changed in API, and some old versions does not support the 2 format of the url
                return CallWS<IEnumerable<ActionPlan>>(requestUrl2, RequestComplexity.Standard);
            }
        }
        #endregion ActionPlan

        #region modules

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <returns></returns>
        IEnumerable<Module> ICastRepsitory.GetModules(string hRef)
        {
            return ListSet<Module>(hRef);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hRef"></param>
        /// <returns></returns>
        Module ICastRepsitory.GetModule(string hRef)
        {
            return Get<Module>(hRef);
        }

        #endregion modules

        #region Results

        IEnumerable<DeltaComponent> ICastRepsitory.GetDeltaComponents(string levelHRef, string snapshotId, string previousSnapshotId, string status, string technology)
        {
            string query = _query_delta_components;
            if (!string.IsNullOrEmpty(technology))
                query = query + "&technologies=({4})";

            // levelHRef can be the application HRef, or module HRef
            // in case of technology, levelHRef should be application HRef
            var requestUrl = string.Format(query, levelHRef, snapshotId, previousSnapshotId, status, technology);

            return CallWS<IEnumerable<DeltaComponent>>(requestUrl, RequestComplexity.Standard);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHRef"></param>
        /// <param name="qualityDistribution"></param>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetComplexityIndicators(string snapshotHRef, string qualityDistribution)
        {
            string relativeUrl = string.Format(_query_result_quality_distribution_complexity, snapshotHRef, qualityDistribution);

            return CallWS<IEnumerable<Result>>(relativeUrl, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetResultsQualityIndicators(string hRef, string qiParam, string snapshotsParam, string modulesParam, string technologiesParam)
        {
            string query = _query_result_quality_indicators;
            if (!string.IsNullOrEmpty(snapshotsParam))
                query = query + "&snapshots=({2})";

            if (!string.IsNullOrEmpty(modulesParam))
                query = query + "&modules=({3})";

            if (!string.IsNullOrEmpty(technologiesParam))
                query = query + "&technologies=({4})";

            string relativeURL = string.Format(query, hRef, qiParam, snapshotsParam, modulesParam, technologiesParam);

            return CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetResultsQualityStandardsRules(string hRef, string stgTagParam, string modulesParam, string technologiesParam)
        {
            string query = _query_result_quality_standards_rules;

            if (!string.IsNullOrEmpty(modulesParam))
                query = query + "&modules=({3})";

            if (!string.IsNullOrEmpty(technologiesParam))
                query = query + "&technologies=({4})";

            string relativeURL = string.Format(query, hRef, stgTagParam, modulesParam, technologiesParam);

            return CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetResultsQualityStandardsTags(string hRef, string stgTagParam)
        {
            string query = _query_quality_standards_evolution;

            string relativeURL = string.Format(query, hRef, stgTagParam);

            return CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<StandardTag> ICastRepsitory.GetQualityStandardsTagsDoc(string hRef)
        {
            string query = _query_quality_standards_information;

            string relativeURL = string.Format(query, hRef);

            return CallWS<IEnumerable<StandardTag>>(relativeURL, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Result> ICastRepsitory.GetResultsSizingMeasures(string hRef, string param, string snapshotsParam, string technologiesParam, string moduleParam)
        {
            string query = _query_result_sizing_measures;

            if (!string.IsNullOrEmpty(snapshotsParam))
                query = query + "&snapshots=({2})";

            if (!string.IsNullOrEmpty(technologiesParam))
                query = query + "&technologies=({3})";

            if (!string.IsNullOrEmpty(moduleParam))
                query = query + "&modules=({4})";

            string relativeURL = string.Format(query, hRef, param, snapshotsParam, technologiesParam, moduleParam);

            return CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }

        IEnumerable<Result> ICastRepsitory.GetResultsBackgroundFacts(string hRef, string param, string snapshotsParam, string technologiesParam, string moduleParam)
        {
            string query = _query_result_background_facts;

            if (!string.IsNullOrEmpty(snapshotsParam))
                query = query + "&snapshots=({2})";

            if (!string.IsNullOrEmpty(technologiesParam))
                query = query + "&technologies=({3})";

            if (!string.IsNullOrEmpty(moduleParam))
                query = query + "&modules=({4})";

            string relativeURL = string.Format(query, hRef, param, snapshotsParam, technologiesParam, moduleParam);

            return CallWS<IEnumerable<Result>>(relativeURL, RequestComplexity.Standard);
        }

        #endregion Result

        #region Configuration

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainHRef"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<QIQualityRules> ICastRepsitory.GetConfQualityRulesBySnapshot(string domainHRef, long snapshotId)
        {
            return ListConfiguration<QIQualityRules>(domainHRef, snapshotId, "quality-rules");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="href"></param>
        /// <returns></returns>
        QIQualityRules ICastRepsitory.GetConfQualityRules(string href)
        {
            return CallWS<QIQualityRules>(href, RequestComplexity.Standard);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainHRef"></param>
        /// <param name="snapshotId"></param>
        /// <returns></returns>
        IEnumerable<QIBusinessCriteria> ICastRepsitory.GetConfBusinessCriteriaBySnapshot(string domainHRef, long snapshotId)
        {                    
            return ListConfiguration<QIBusinessCriteria>(domainHRef, snapshotId, "business-criteria");
        }

        QIBusinessCriteria ICastRepsitory.GetConfBusinessCriteria(string href)
        {
            return CallWS<QIBusinessCriteria>(href, RequestComplexity.Standard);
        }
        #endregion Configuration

        #region PRIVATE

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeURL"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        private T CallWS<T>(string relativeURL, RequestComplexity pComplexity) where T : class
        {
             var requestUrl = _CurrentConnection.EndsWith("/") ? _CurrentConnection.Substring(0, _CurrentConnection.Length - 1) : _CurrentConnection;
            requestUrl += "/";
            requestUrl += relativeURL.StartsWith("/") ? relativeURL.Substring(1) : relativeURL;

            try
            {
                var jsonString = _Client.DownloadString(requestUrl, pComplexity);

                var serializer = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
                try
                {
                    T res = serializer.ReadObject(ms) as T;
                    return res;
                }
                finally
                {
                    ms.Close();
                }
                
            }
            catch (WebException e)
            {
                LogHelper.Instance.LogError(e.Message);
                return null;
            }
        }

        private string CallWSJsonOnly(string relativeURL, RequestComplexity pComplexity)
        {
            var requestUrl = _CurrentConnection.EndsWith("/") ? _CurrentConnection.Substring(0, _CurrentConnection.Length - 1) : _CurrentConnection;
            requestUrl += "/";
            requestUrl += relativeURL.StartsWith("/") ? relativeURL.Substring(1) : relativeURL;
            var jsonString = string.Empty;
            try
            {
                jsonString = _Client.DownloadString(requestUrl, pComplexity);
            }
            catch (WebException e)
            {
                LogHelper.Instance.LogError(e.Message);
            }
            
            return jsonString;
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

            try
            {
                var csvString = _Client.DownloadCsvString(requestUrl, pComplexity);
                var serializer = new CsvSerializer<T>();
                return serializer.ReadObjects(csvString, count, PropNames);
            }
            catch (WebException e)
            {
                LogHelper.Instance.LogError(e.Message);
                return null;
            }

        }

        private string CallStringWS(string relativeURL, RequestComplexity pComplexity)
        {
            var requestUrl = _CurrentConnection.EndsWith("/") ? _CurrentConnection.Substring(0, _CurrentConnection.Length - 1) : _CurrentConnection;
            requestUrl += "/";
            requestUrl += relativeURL.StartsWith("/") ? relativeURL.Substring(1) : relativeURL;

            try
            {
                return _Client.DownloadPlainText(requestUrl, pComplexity);
            }
            catch (WebException e)
            {
                LogHelper.Instance.LogError(e.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pRelativeUrl"></param>
        /// <returns></returns>
        private IEnumerable<T> ListSet<T>(string pRelativeUrl) where T : class
        {
            string typeName = typeof(T).Name.ToLower();
            string setName = typeName.EndsWith("s", StringComparison.InvariantCultureIgnoreCase) ? typeName : typeName + "s";

            return ListSet<T>(pRelativeUrl, setName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pRelativeUrl"></param>
        /// <param name="pSetName"></param>
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
        /// <param name="setName"></param>
        /// <returns></returns>
        private IEnumerable<T> ListConfiguration<T>(string domainHRef, long snapshotId, string setName) where T : class
        {

            string relativeURL = string.Format(_query_configuration, domainHRef, snapshotId);

            return ListSet<T>(relativeURL, setName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private T Get<T>(string id) where T : class
        {
             return CallWS<T>(id, RequestComplexity.Standard);                     
        }

        #endregion

      
    }
}
