using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "codeFragment")]
    public class CodeFragment
    {
        [DataMember(Name = "file")]
        public CodeFile CodeFile { get; set; }

        [DataMember(Name = "endColumn")]
        public int? EndColumn { get; set; }

        [DataMember(Name = "endLine")]
        public int EndLine { get; set; }

        [DataMember(Name = "startColumn")]
        public int? StartColumn { get; set; }

        [DataMember(Name = "startLine")]
        public int StartLine { get; set; }


    }
}