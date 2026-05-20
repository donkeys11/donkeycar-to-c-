using System.Text.Json.Serialization;

namespace DonkeycarManager
{
    public class DonkeyFrame
    {
        [JsonPropertyName("_index")]
        public int Index { get; set; }

        [JsonPropertyName("_session_id")]
        public string SessionId { get; set; } = "";

        [JsonPropertyName("_timestamp_ms")]
        public long TimestampMs { get; set; }

        [JsonPropertyName("cam/image_array")]
        public string ImageFileName { get; set; } = "";

        [JsonPropertyName("user/angle")]
        public double Angle { get; set; }

        [JsonPropertyName("user/mode")]
        public string Mode { get; set; } = "";

        [JsonPropertyName("user/throttle")]
        public double Throttle { get; set; }
    }
}  