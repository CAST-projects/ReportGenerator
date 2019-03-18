using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "bookmarks")]
    public class CodeBookmark
    {
        [DataMember(Name = "component")]
        public Component Component { get; set; }

        [DataMember(Name = "codeFragment")]
        public CodeFragment CodeFragment { get; set; }

    }
}