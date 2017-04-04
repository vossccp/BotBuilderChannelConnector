using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    // see: https://developers.facebook.com/docs/messenger-platform/send-api-reference/generic-template

    public class FacebookElement
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("buttons")]
        public List<FacebookButton> Buttons { get; set; }
        [JsonProperty("default_action")]
        public FacebookButton DefaultAction { get; set; }
    }
}