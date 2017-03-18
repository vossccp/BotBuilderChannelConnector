using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}