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
using System.Diagnostics;
using System.Net;
using System.Text;
using Cast.Util.Log;
using CastReporting.Mediation.Interfaces;
using CastReporting.Mediation.Properties;


namespace CastReporting.Mediation
{

   

    /// <summary>
    /// WebClient Class for Cast Reporting
    /// </summary>
    public class CastProxy : System.Net.WebClient, ICastProxy
    {     
        #region ATTRIBUTES

        /// <summary>
        /// 
        /// </summary>
        private RequestComplexity _CurrentComplexity = RequestComplexity.Standard;      
        
        #endregion ATTRIBUTES

        #region PROPERTIES
        
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout
        {
            get { return GetRequestTimeOut(_CurrentComplexity); }
        }
        
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
            string credentials = this.CreateBasicAuthenticationCredentials(login, password);
            base.Headers.Add(System.Net.HttpRequestHeader.Authorization, credentials);
        }
            

        #endregion CONSTRUCTORS

        private string DownloadContent(string pUrl, string mimeType, RequestComplexity pComplexity)
        {

            string result = string.Empty;

            try
            {
                base.Headers.Add(System.Net.HttpRequestHeader.Accept, mimeType);
                base.Encoding = Encoding.UTF8;

                RequestComplexity previousComplexity = this._CurrentComplexity;
                this._CurrentComplexity = pComplexity;

                var requestWatch = new Stopwatch();
                requestWatch.Start();
                result = base.DownloadString(pUrl);
                requestWatch.Stop();

                this._CurrentComplexity = previousComplexity;

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

                 throw ex;
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
                       
            result.Timeout = this.Timeout;
            
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
        private int GetRequestTimeOut(RequestComplexity pComplexity)
        {
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
                case RequestComplexity.Standard:
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
        private string CreateBasicAuthenticationCredentials(string userName, string password)
        {
            string returnValue = string.Empty;

            string base64UsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes(String.Format("{0}:{1}", userName, password)));

            returnValue = String.Format("Basic {0}", base64UsernamePassword);

            return returnValue;
        }

        #endregion METHODS
    }
}
