using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookQuickReply
    {
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}