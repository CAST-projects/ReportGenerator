
using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract(Name = "omg-functions-functional-technical")]
    public class OmgFunctionTechnical
    {
        [DataMember(Name = "Object id")]
        public string ObjectId { get; set; }

        [DataMember(Name = "Object name")]
        public string ObjectName { get; set; }

        [DataMember(Name = "Object name location")]
        public string ObjectFullName { get; set; }

        [DataMember(Name = "Object type")]
        public string ObjectType { get; set; }


        [DataMember(Name = "Object status")]
        public string ObjectStatus { get; set; }

        [DataMember(Name = "Effort complexity")]
        public string EffortComplexity { get; set; }

        [DataMember(Name = "Equivalence ratio")]
        public string EquivalenceRatio { get; set; }

        [DataMember(Name = "AEP")]
        public string AepCount { get; set; }

    }
}
