using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "diagnosis")]
    public class Diagnosis
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

    }
}