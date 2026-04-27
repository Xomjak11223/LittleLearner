using System.Text.Json.Serialization;

namespace LittleLearner.LCS
{
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(object))]
    internal partial class HybridSampleJsonContext : JsonSerializerContext
    {
    }
}
