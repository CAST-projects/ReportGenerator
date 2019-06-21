using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "associatedValue")]
    public class AssociatedValue
    {
        [DataMember(Name = "bookmarks")]
        public CodeBookmark[][] Bookmarks { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "values")]
        public object[] Values { get; set; }

    }
}