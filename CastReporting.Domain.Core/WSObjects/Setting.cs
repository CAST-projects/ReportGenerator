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
using System.Linq;

namespace CastReporting.Domain
{
    /// <summary>
    /// Represents a central database.
    /// </summary>
    [Serializable]
    public class Setting 
    {
        /// <summary>
        /// 
        /// </summary>       
        private ReportingParameter _reportingParameter;
        public ReportingParameter ReportingParameter
        {
            get => _reportingParameter ?? (_reportingParameter = new ReportingParameter());
            set => _reportingParameter = value;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<WSConnection> _wsConnections;
        public List<WSConnection> WSConnections 
        {
            get => _wsConnections ?? (_wsConnections = new List<WSConnection>());
            set => _wsConnections = value;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WSConnection GetActiveConnection()
        {
            return WSConnections.FirstOrDefault(_ => _.IsActive);                
        }


        /// <summary>
        /// 
        /// </summary>
        public void ChangeActiveConnection(string newActiveUrl)
        {
            WSConnection previousActiveconnection= WSConnections.FirstOrDefault(_ => _.IsActive);
            if (previousActiveconnection!=null) previousActiveconnection.IsActive = false;

            WSConnection newActiveConnection = WSConnections.FirstOrDefault(_ => _.Url.Equals(newActiveUrl));
            if (newActiveConnection != null) newActiveConnection.IsActive = true;
        }
    }
}
