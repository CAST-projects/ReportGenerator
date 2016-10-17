using System;
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract]
    public class RuleDetails
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "key")]
        public int? Key { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "compoundedWeight")]
        public int? CompoundedWeight { get; set; }

        [DataMember(Name = "critical")]
        public bool Critical { get; set; }

        [DataMember(Name = "compoundedWeightFormula")]
        public string CompoundedWeightFormula { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Key, Name, Href);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is RuleDetails)) return false;


            return Href != null &&
                   (obj as RuleDetails).Href != null &&
                   Href.Equals((obj as RuleDetails).Href);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Href != null ? Href.GetHashCode() : String.Empty.GetHashCode();
        }

    }
}
