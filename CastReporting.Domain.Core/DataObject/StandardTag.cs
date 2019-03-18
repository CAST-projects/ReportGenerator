using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract]
    public class StandardTag
    {
        [DataMember(Name = "standard")]
        public string Href { get; set; }

        [DataMember(Name = "id")]
        public string Key { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "reference")]
        public string Reference { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
