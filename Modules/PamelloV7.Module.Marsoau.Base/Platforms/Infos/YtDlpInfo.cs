using System.Text.Json.Serialization;

namespace PamelloV7.Module.Marsoau.Base.Platforms.Infos;

public class YtDlpInfo {
    [JsonPropertyName("id")]            public string Id { get; set; } = "";
    [JsonPropertyName("title")]         public string Title { get; set; } = "";
    [JsonPropertyName("thumbnail")]     public string? Thumbnail { get; set; }
    [JsonPropertyName("thumbnails")]    public List<Thumbnail>? Thumbnails { get; set; }
    [JsonPropertyName("duration")]      public double? Duration { get; set; }
    [JsonPropertyName("uploader")]      public string? Uploader { get; set; }
    [JsonPropertyName("channel")]       public string? Channel { get; set; }
    [JsonPropertyName("webpage_url")]   public string? WebpageUrl { get; set; }
    [JsonPropertyName("extractor")]     public string? Extractor { get; set; }
    [JsonPropertyName("chapters")]      public List<Chapter>? Chapters { get; set; }
}

public class Thumbnail {
    [JsonPropertyName("url")]        public string Url { get; set; } = "";
    [JsonPropertyName("width")]      public int? Width { get; set; }
    [JsonPropertyName("height")]     public int? Height { get; set; }
    [JsonPropertyName("preference")] public int? Preference { get; set; }
}

public class Chapter {
    [JsonPropertyName("start_time")] public double StartTime { get; set; }
    [JsonPropertyName("end_time")]   public double EndTime { get; set; }
    [JsonPropertyName("title")]      public string Title { get; set; } = "";
}
