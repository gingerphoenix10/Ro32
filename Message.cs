using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ro32;

public class Message
{
    [JsonPropertyName("command")]
    public string Command { get; set; } = null!;

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}