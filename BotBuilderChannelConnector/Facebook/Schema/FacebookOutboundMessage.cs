using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBuilder.ChannelConnector.Facebook.Schema
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
