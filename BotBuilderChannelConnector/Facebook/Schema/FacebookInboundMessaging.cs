using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookInboundMessaging
    {
        [JsonProperty("sender")]
        public FacebookAccount Sender { get; set; }
        [JsonProperty("recipient")]
        public FacebookAccount Recipient { get; set; }
        [JsonProperty("timestamp")]
        public long? Timestamp { get; set; }
        [JsonProperty("message")]
        public FacebookInboundMessage Message { get; set; }
    }
}