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
            return $"{Key}-{Name}-{Href}";
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
                   ((RuleDetails) obj).Href != null &&
                   Href.Equals(((RuleDetails) obj).Href);
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

    }
}
