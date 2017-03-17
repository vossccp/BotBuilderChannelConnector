using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookButton
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("payload")]
        public string Payload { get; set; }
        [JsonProperty("messenger_extension")]
        public bool? MessengerExtension { get; set; }
        [JsonProperty("webview_height_ratio")]
        public string WebviewHeightRatio { get; set; }
        [JsonProperty("fallback_url")]
        public string FallbackUrl { get; set; }
    }
}