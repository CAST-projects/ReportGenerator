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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "applicationResult")]
    public class ApplicationResult
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "reference")]
        public ResultReference Reference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "result")]
        public ResultDetail DetailResult { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "moduleResults")]
        public ModuleResult[] ModulesResult { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "technologyResults")]
        public TechnologyResult[] TechnologyResult { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ApplicationResult> CriticalRulesViolation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ApplicationResult> NonCriticalRulesViolation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ApplicationResult> RulesViolation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Transaction> Transactions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Component> Components { get; set; }
    }
}
