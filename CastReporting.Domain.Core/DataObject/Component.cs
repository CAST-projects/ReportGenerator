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
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "component")]
    public class Component
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "name")]
        public string Name {get;set;}

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "shortName")]
        public string ShortName { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "propagationRiskIndex")]
        public double PropagationRiskIndex { get; set; }

        [DataMember(Name = "priBusinessCriterion")]
        public string PriBusinessCriterion { get; set; }

        public string GetComponentId()
        {
            return Href.Split('/')[2];
        }

    }
}
