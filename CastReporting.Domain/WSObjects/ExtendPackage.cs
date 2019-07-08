
using System.Runtime.Serialization;

namespace CastReporting.Domain.WSObjects
{
    [DataContract(Name = "package")]
    public class ExtendPackage
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }

    }
}
