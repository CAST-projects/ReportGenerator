/*
 *   Copyright (c) 2015 CAST
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
using System.ComponentModel;
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

        
        [DataMember(Name = "technologies")]
        public string[] Technologies { get; set; }

        #endregion PROPERTIES

        public string DomainId
        {
            get
            {
                return Href.Split('/').FirstOrDefault();
            }
        }

        #region METHODS
        public override string ToString()
        {
            return string.Format("{0}", Name);
        }


        public override bool Equals(object obj)
        {
            if(!(obj is CRObject)) return false;


            return Href != null && 
                   (obj as CRObject).Href!=null  && 
                   Href.Equals((obj as CRObject).Href);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Href!=null? Href.GetHashCode():String.Empty.GetHashCode();
        }

        #endregion METHODS
    }
}
