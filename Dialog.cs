using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ro32;

public class Dialog
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("text")]
    public string Text { get; set; } = null!;
}