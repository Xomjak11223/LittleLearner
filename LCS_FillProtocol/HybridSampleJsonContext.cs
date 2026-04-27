using System.Text.Json.Serialization;

namespace LCS_FillProtocol
{
    [JsonSerializable(typeof(string))]
    internal partial class HybridSampleJsonContext : JsonSerializerContext
    {
    }
}
