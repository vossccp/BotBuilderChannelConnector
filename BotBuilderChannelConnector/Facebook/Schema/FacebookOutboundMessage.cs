using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookOutboundMessage
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("quick_replies")]
        public List<FacebookQuickReply> QuickReplies { get; set; }
        [JsonProperty("attachment")]
        public FacebookAttachment Attachment { get; set; }
    }
}
