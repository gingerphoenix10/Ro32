using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ro32;

public class Window
{
    [JsonPropertyName("type")]
    public string setType { get; set; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }
}

public class Float
{
    [JsonPropertyName("PositionX")]
    public int PositionX { get; set; }
    [JsonPropertyName("PositionY")]
    public int PositionY { get; set; }
    [JsonPropertyName("Width")]
    public int Width { get; set; }
    [JsonPropertyName("Height")]
    public int Height { get; set; }
}