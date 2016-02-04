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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


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
        private ReportingParameter _ReportingParameter;
        public ReportingParameter ReportingParameter
        {
            get
            {
                if (_ReportingParameter == null) _ReportingParameter = new ReportingParameter();

                return _ReportingParameter;
            }
            set
            {
                _ReportingParameter = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<WSConnection> _WSConnections;
        public List<WSConnection> WSConnections 
        {
            get
            {
                if (_WSConnections == null) _WSConnections = new List<WSConnection>();
                
                return _WSConnections;
            }
            set
            {
                _WSConnections = value;
            }
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
        public void ChangeActiveConnection(string NewActiveUrl)
        {
            WSConnection previousActiveconnection= WSConnections.FirstOrDefault(_ => _.IsActive);
            if (previousActiveconnection!=null) previousActiveconnection.IsActive = false;

            WSConnection newActiveConnection = WSConnections.FirstOrDefault(_ => _.Url.Equals(NewActiveUrl));
            newActiveConnection.IsActive = true;
     
        }
    }
}
