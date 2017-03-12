using Newtonsoft.Json;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("payload")]
        public FacebookPayload Payload { get; set; }
    }
}