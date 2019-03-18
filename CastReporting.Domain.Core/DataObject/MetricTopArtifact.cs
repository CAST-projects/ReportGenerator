using System.Runtime.Serialization;

namespace CastReporting.Domain
{
    [DataContract(Name = "violations")]
    public class MetricTopArtifact
    {
        [DataMember(Name = "Quality rule name")]
        public string QualityRuleName { get; set; }

        [DataMember(Name = "Object name location")]
        public string ObjectNameLocation { get; set; }

        [DataMember(Name = "Object status")]
        public string ObjectStatus { get; set; }

        [DataMember(Name = "Snapshot date")]
        public string SnapshotDate { get; set; }

        [DataMember(Name = "Diagnosis findings")]
        public string DiagnosisFindings { get; set; }

        [DataMember(Name = "Violation status")]
        public string Violationstatus { get; set; }

        [DataMember(Name = "Action plan status")]
        public string ActionPlanStatus { get; set; }

        [DataMember(Name = "Action plan priority")]
        public string ActionPlanPriority { get; set; }
    }
}
