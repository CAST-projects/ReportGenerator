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
using System.Text;
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// Represent an analysed application.
    /// </summary>
    [DataContract(Name = "application")]
    public class Application : CRObject
    {      
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Result> BusinessCriteriaResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Result> SizingMeasuresResults { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Snapshot> Snapshots { get; set; }

        public IEnumerable<StandardTag> StandardTags { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<System> Systems { get; set; }
        
        public string SystemNames {
        	get {
        		var sb = new StringBuilder();
	            if (Systems == null) return sb.ToString();

	            foreach (var sys in Systems) {
	                if (sb.Length > 0)
	                    sb.Append(", ");
	                sb.Append(sys.Name);
	            }
	            return sb.ToString();
        	}
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }
    }
}
