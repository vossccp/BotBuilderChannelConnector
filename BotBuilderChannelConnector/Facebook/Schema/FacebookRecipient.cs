using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookRecipient
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}