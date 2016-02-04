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
using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using CastReporting.Repositories;
using CastReporting.Repositories.Interfaces;

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class CastDomainBLL : BaseBLL
    {    
  
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSnapshot"></param>
        /// <param name="previousSnapshot"></param>
        public CastDomainBLL(WSConnection connection)
            : base(connection)
        {
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CastDomain> GetDomains()
        {
            using (var castRepsitory = GetRepository())
            {
                return castRepsitory.GetDomains();
            }
        }

           
        /// <summary>
        /// 
        /// </summary>
        public List<Application> GetApplications()
        {
            List<Application> applications = new List<Application>();

            var domains = GetDomains();

            using (var castRepsitory = GetRepository())
            {
                foreach (var domain in domains)
                {
					var domainApps = castRepsitory.GetApplicationsByDomain(domain.Href);

                    foreach (var app in domainApps)
                    {
						if (string.IsNullOrEmpty(app.Version)) {
                        app.Version = domain.Version;
                    }
                }
					
                    applications.AddRange(domainApps);
                }
            }

           return applications.OrderBy(_ => _.Name).ToList();
        }
    }
}
