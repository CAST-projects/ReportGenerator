
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract(Name = "omg-functions-functional")]
    public class OmgFunction
    {
        [DataMember(Name = "Element Type")]
        public string ElementType { get; set; }

        [DataMember(Name = "Function Name")]
        public string FunctionName { get; set; }

        [DataMember(Name = "Object Name")]
        public string ObjectName { get; set; }

        [DataMember(Name = "No. FPs")]
        public string NoOfFPs { get; set; }

        [DataMember(Name = "AEP")]
        public string Aeps { get; set; }

        [DataMember(Name = "Complexity Factor")]
        public string ComplexityFactor { get; set; }

        [DataMember(Name = "Updated Artifacts")]
        public string UpdatedArtifacts { get; set; }

        [DataMember(Name = "Object Type")]
        public string ObjectType { get; set; }

        [DataMember(Name = "Module Name")]
        public string ModuleName { get; set; }

        [DataMember(Name = "Technology")]
        public string Technology { get; set; }
    }
}
