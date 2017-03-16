using Newtonsoft.Json;

namespace BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookSender
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}