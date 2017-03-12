using Newtonsoft.Json;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookRecipient
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}