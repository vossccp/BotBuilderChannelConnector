using Newtonsoft.Json;
using System.Collections.Generic;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookPayload
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("template_type")]
        public string TemplateType { get; set; }
        [JsonProperty("elements")]
        public List<FacebookElement> Elements { get; set; }
    }
}