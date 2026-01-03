using System.Text.Json.Serialization;
using Portfolio.API.Utils;

namespace Portfolio.API.Models;
public class ProjectItem
{
    [JsonPropertyName("orderid")]
    public int OrderID { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("languages")]
    [JsonConverter(typeof(StringOrArrayConverter))]
    public IEnumerable<string> Languages { get; set; } = new List<string>();

    [JsonPropertyName("backend")]
    [JsonConverter(typeof(StringOrArrayConverter))]
    public IEnumerable<string> Backend { get; set; } = new List<string>();

    [JsonPropertyName("frontend")]
    [JsonConverter(typeof(StringOrArrayConverter))]
    public IEnumerable<string> Frontend { get; set; } = new List<string>();

    [JsonPropertyName("database")]
    [JsonConverter(typeof(StringOrArrayConverter))]
    public IEnumerable<string> Database { get; set; } = new List<string>();

    // JSON usa "image" singular → mapear para Images
    [JsonPropertyName("images")]
    [JsonConverter(typeof(StringOrArrayConverter))]
    public IEnumerable<string> Images { get; set; } = new List<string>();

    [JsonPropertyName("github")]
    public string github { get; set; } = string.Empty;

    [JsonPropertyName("link")]
    public string link { get; set; } = string.Empty;
}
