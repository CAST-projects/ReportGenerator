
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract(Name = "ifpug-functions")]
    public class IfpugFunction
    {
        [DataMember(Name = "Element Type")]
        public string ElementType { get; set; }

        [DataMember(Name = "Object Name")]
        public string ObjectName { get; set; }

        [DataMember(Name = "Nb of FPs")]
        public string NbOfFPs { get; set; }

        [DataMember(Name = "No. FPs")]
        public string NoOfFPs { get; set; }

        [DataMember(Name = "AFP")]
        public string Afps { get; set; }

        [DataMember(Name = "EFP")]
        public string Efps { get; set; }

        [DataMember(Name = "FP Details")]
        public string FPDetails { get; set; }

        [DataMember(Name = "Object Type")]
        public string ObjectType { get; set; }

        [DataMember(Name = "Module Name")]
        public string ModuleName { get; set; }

        [DataMember(Name = "Technology")]
        public string Technology { get; set; }
    }
}
