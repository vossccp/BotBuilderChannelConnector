using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookPayload
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("template_type")]
        public string TemplateType { get; set; }
        [JsonProperty("top_element_style")]
        public string TopElementStyle { get; set; }
        [JsonProperty("elements")]
        public List<FacebookElement> Elements { get; set; }
        [JsonProperty("buttons")]
        public List<FacebookButton> Buttons { get; set; }
        [JsonProperty("coordinates")]
        public FacebookCoordinates Coordinates { get; set; }
    }
}