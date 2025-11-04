using System.Runtime.Serialization;

namespace LazyGatherer.Models;

public enum ComparerEnum
{
    [EnumMember(Value = "Max yield")]
    MaxYield,

    [EnumMember(Value = "Max Yield per GP")]
    MaxYieldPerGp
}
