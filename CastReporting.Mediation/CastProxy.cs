/*
 *   Copyright (c) 2018 CAST
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
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using Cast.Util.Log;
using CastReporting.Mediation.Interfaces;
using CastReporting.Mediation.Properties;

namespace CastReporting.Mediation
{

   

    /// <summary>
    /// WebClient Class for Cast Reporting
    /// </summary>
    public class CastProxy : WebClient, ICastProxy
    {     
        #region ATTRIBUTES

        /// <summary>
        /// 
        /// </summary>
        private RequestComplexity _currentComplexity = RequestComplexity.Standard;      
        
        #endregion ATTRIBUTES

        #region PROPERTIES
        
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout => GetRequestTimeOut(_currentComplexity);

        /// <summary>
        /// HEAD method option
        /// </summary>
        public bool HeadOnly { get; set; }

        #endregion PROPERTIES

        #region CONSTRUCTORS

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CastProxy(string login, string password)
        {
            string credentials = CreateBasicAuthenticationCredentials(login, password);
            Headers.Add(HttpRequestHeader.Authorization, credentials);
        }

        #endregion CONSTRUCTORS

        private string DownloadContent(string pUrl, string mimeType, RequestComplexity pComplexity)
        {

            string result;

            try
            {
                Headers.Add(HttpRequestHeader.Accept, mimeType);
                var culture = Thread.CurrentThread.CurrentCulture;
                Headers.Remove(HttpRequestHeader.AcceptLanguage);
                Headers.Add(HttpRequestHeader.AcceptLanguage, culture.Name.Equals("zh-CN") ? "zh" : "en");

                Encoding = Encoding.UTF8;

                RequestComplexity previousComplexity = _currentComplexity;
                _currentComplexity = pComplexity;

                var requestWatch = new Stopwatch();
                requestWatch.Start();
                result = DownloadString(pUrl);
                requestWatch.Stop();

                _currentComplexity = previousComplexity;

                LogHelper.Instance.LogDebugFormat
                        ("Request URL '{0}' - Time elapsed : {1} "
                        , pUrl
                        , requestWatch.Elapsed.ToString()
                        );
               
            }
            catch(Exception ex)
            {
                 LogHelper.Instance.LogErrorFormat
                        ("Request URL '{0}' - Error execution :  {1}"
                        , pUrl
                        , ex.Message
                        );

                 throw;
            }

            return result;
        }
        #region METHODS

        /// <summary>
        /// Download String
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadString(string pUrl, RequestComplexity pComplexity)
        {
        	return DownloadContent(pUrl, "application/json", pComplexity);
        }

        public string DownloadPlainText(string pUrl, RequestComplexity pComplexity)
        {
            return DownloadContent(pUrl, "text/plain", pComplexity);
        }


        /// <summary>
        /// Download String by URI
        /// </summary>
        /// <param name="pUri"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadString(Uri pUri, RequestComplexity pComplexity)
        {
            return DownloadString(pUri.ToString(), pComplexity);         
        }

        /// <summary>
        /// Download Csv String
        /// </summary>
        /// <param name="pUrl"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadCsvString(string pUrl, RequestComplexity pComplexity)
        {
            try
            {
                return DownloadContent(pUrl, "text/csv", pComplexity);
            }
            catch (WebException webEx)
            {
                // AIP < 8 sends CSV data as application/vnd.ms-excel
                LogHelper.Instance.LogInfo(webEx.Message);
                return DownloadContent(pUrl, "application/vnd.ms-excel", pComplexity);
            }
        }

        /// <summary>
        /// Download String by URI
        /// </summary>
        /// <param name="pUri"></param>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        public string DownloadCsvString(Uri pUri, RequestComplexity pComplexity)
        {
            return DownloadCsvString(pUri.ToString(), pComplexity);
        }

        /// <summary>
        /// Get Web Request
        /// </summary>
        /// <param name="pAddress"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri pAddress)
        {
            var result = base.GetWebRequest(pAddress);

            if (result == null) return null;
            result.Timeout = Timeout;

            if (HeadOnly && result.Method == "GET")
            {
                result.Method = "HEAD";
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pComplexity"></param>
        /// <returns></returns>
        private static int GetRequestTimeOut(RequestComplexity pComplexity)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (pComplexity)
            {
                case RequestComplexity.Long: 
                    {
                        return Settings.Default.TimoutLong; 
                    }
                case RequestComplexity.Soft: 
                    {
                        return Settings.Default.TimeoutQuick; 
                    }
                //case RequestComplexity.Standard:
                default: 
                    {
                        return Settings.Default.TimeoutStandard; 
                    }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string CreateBasicAuthenticationCredentials(string userName, string password)
        {
            string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}"));

            var returnValue = $"Basic {base64UsernamePassword}";

            return returnValue;
        }

        #endregion METHODS
    }
}
