using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract]
    public class Contributor
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "key")]
        public int? Key { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "weight")]
        public int? Weight { get; set; }

        [DataMember(Name = "critical")]
        public bool Critical { get; set; }

    }
}
