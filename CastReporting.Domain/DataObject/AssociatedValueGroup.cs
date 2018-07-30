using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "associatedValue")]
    public class AssociatedValueGroup
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "values")]
        public CodeBookmark[][] Values { get; set; }

    }
}