using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract]
    public class RuleDescription
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        public string Technologies { get; set; }

        [DataMember(Name = "rationale")]
        public string Rationale { get; set; }

        [DataMember(Name = "reference")]
        public string Reference { get; set; }

        [DataMember(Name = "remediation")]
        public string Remediation { get; set; }

        [DataMember(Name = "output")]
        public string Output { get; set; }

        [DataMember(Name = "associatedValueName")]
        public string AssociatedValueName { get; set; }

        [DataMember(Name = "total")]
        public string Total { get; set; }

        [DataMember(Name = "sample")]
        public string Sample { get; set; }

        [DataMember(Name = "remediationSample")]
        public string RemediationSample { get; set; }
    }
}
