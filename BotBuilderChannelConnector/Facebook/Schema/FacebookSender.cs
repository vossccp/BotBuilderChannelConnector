using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookSender
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}