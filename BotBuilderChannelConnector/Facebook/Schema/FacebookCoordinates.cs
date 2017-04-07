using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookCoordinates
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("long")]
        public double Long { get; set; }
    }
}
