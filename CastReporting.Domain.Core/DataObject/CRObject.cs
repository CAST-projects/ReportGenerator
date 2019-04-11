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
using System.Linq;
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// Represents the base class of all Cast Reporting objects.
    /// </summary>
    [Serializable]
    [DataContract()]
    public abstract class CRObject
    {
        #region PROPERTIES

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "adgWebSite")]
        public string AdgWebSite { get; set; }


        [DataMember(Name = "adgDatabase")]
        public string AdgDatabase { get; set; }


        [DataMember(Name = "adgLocalId")]
        public string AdgLocalId { get; set; }


        [DataMember(Name = "adgVersion")]
        public string AdgVersion { get; set; }


        [DataMember(Name = "technologies")]
        public string[] Technologies { get; set; }

        #endregion PROPERTIES

        public string DomainId => Href.Split('/').FirstOrDefault();

        #region METHODS
        public override string ToString()
        {
            return $"{Name}";
        }


        public override bool Equals(object obj)
        {
            if(!(obj is CRObject)) return false;


            return Href != null && 
                   ((CRObject) obj).Href!=null  && 
                   Href.Equals(((CRObject) obj).Href);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Href?.GetHashCode() ?? string.Empty.GetHashCode();
        }

        #endregion METHODS
    }
}
