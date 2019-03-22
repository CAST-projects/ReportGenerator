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
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "date")]
    public class CastDate:IComparable
    {
        /// <summary>
        /// 
        /// </summary>        
        private double? _time;
        [DataMember(Name = "time")]
        public double? Time 
        {
            set
            {
                _time = value;

                DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
                DateSnapShot = _time.HasValue ? date.AddMilliseconds(_time.Value).ToLocalTime() : (DateTime?)null;
            }
            get => _time;
        }


        /// <summary>
        /// Get/Set the snapshot date
        /// </summary>

        public DateTime? DateSnapShot 
        { 
            get; 
            private set; 
        }

        public int CompareTo(object obj)
        {
            var dt = (obj as CastDate);


            return dt != null && DateSnapShot.HasValue && dt.DateSnapShot.HasValue ? DateTime.Compare(DateSnapShot.Value, dt.DateSnapShot.Value) : 1;
        }
    }

}