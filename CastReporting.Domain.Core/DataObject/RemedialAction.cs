using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "remedialAction")]
    public class RemedialAction
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "priority")]
        public string Priority { get; set; }

        [DataMember(Name = "comment")]
        public string Comment { get; set; }

        [DataMember(Name = "dates")]
        public RemedialDates Dates { get; set; }
        
    }
}