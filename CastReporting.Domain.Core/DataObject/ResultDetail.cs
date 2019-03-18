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
    [DataContract(Name = "resultDetail")]
    public class ResultDetail
    {
        /// <summary>
        /// Used by Quality indicators
        /// </summary>
        [DataMember(Name = "grade")]
        public double? Grade {get;set;}

        /// <summary>
        /// Used by sizing measures
        /// </summary>
        [DataMember(Name = "value")]
        public double? Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "categories")]
        public Category[] Categories { get; set; }

        /// <summary>
        /// used by Rules
        /// </summary>
        [DataMember(Name = "violationRatio")]
        public ViolationRatio ViolationRatio { get; set; }

        /// <summary>
        /// used by Rules
        /// </summary>
        [DataMember(Name = "evolutionSummary")]
        public EvolutionSummary EvolutionSummary { get; set; }
        
    }
}
