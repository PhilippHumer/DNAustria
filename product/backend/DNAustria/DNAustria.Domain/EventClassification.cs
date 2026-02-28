using System.Runtime.Serialization;

namespace DNAustria.Domain;

public enum EventClassification
{
    [EnumMember(Value = "Scheduled")]
    Scheduled,

    [EnumMember(Value = "On-Demand")]
    OnDemand
}