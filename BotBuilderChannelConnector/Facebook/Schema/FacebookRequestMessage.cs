using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;

namespace BotBuilder.ChannelConnector.Facebook.Schema
{
    // This is the message, facebook sends to us
    public class FacebookRequestMessage
    {
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("entry")]
        public FacebookEntry[] Entries { get; set; }
    }
}