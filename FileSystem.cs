using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ro32;
public class FilesystemCommand
{
    [JsonPropertyName("type")]
    public string setType { get; set; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }
}

public class FilesystemCreate
{
    [JsonPropertyName("path")]
    public string FilePath { get; set; }

    [JsonPropertyName("textData")]
    public string TextData { get; set; }
}