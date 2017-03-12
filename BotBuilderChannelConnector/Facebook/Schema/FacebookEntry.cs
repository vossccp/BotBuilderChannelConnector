using Newtonsoft.Json;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("time")]
        public long Time { get; set; }
        [JsonProperty("messaging")]
        public FacebookInboundMessaging[] Messaging { get; set; }
    }
}