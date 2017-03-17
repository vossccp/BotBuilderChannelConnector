using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookInboundMessaging
    {
        [JsonProperty("sender")]
        public FacebookSender Sender { get; set; }
        [JsonProperty("recipient")]
        public FacebookRecipient Recipient { get; set; }
        [JsonProperty("timestamp")]
        public long? Timestamp { get; set; }
        [JsonProperty("message")]
        public FacebookInboundMessage Message { get; set; }
    }
}