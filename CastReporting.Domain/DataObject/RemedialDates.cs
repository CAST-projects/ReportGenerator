using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "dates")]
    public class RemedialDates
    {
        [DataMember(Name = "updated")]
        public CastDate Updated { get; set; }

    }
}