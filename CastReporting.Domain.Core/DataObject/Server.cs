using System.Runtime.Serialization;

namespace CastReporting.Domain.Core.DataObject
{
    [DataContract]
    public class Server
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }
    }
}