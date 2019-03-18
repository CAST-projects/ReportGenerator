using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "violations")]
    public class Violation
    {
        [DataMember(Name = "component")]
        public Component Component { get; set; }

        [DataMember(Name = "diagnosis")]
        public Diagnosis Diagnosis { get; set; }

        [DataMember(Name = "rulePattern")]
        public RulePattern RulePattern { get; set; }

        [DataMember(Name = "exclusionRequest")]
        public ExclusionRequest ExclusionRequest { get; set; }

        [DataMember(Name = "remedialAction")]
        public RemedialAction RemedialAction { get; set; }
    }
}