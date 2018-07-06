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

    }
}