using Newtonsoft.Json;

namespace BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookRecipient
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}