using Newtonsoft.Json;

namespace BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookInboundMessage
    {
        [JsonProperty("mid")]
        public string Mid { get; set; }
        [JsonProperty("seq")]
        public int? Seq { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("quick_reply")]        
        public FacebookQuickReply QuickReply { get; set; }
        [JsonProperty("Attachments")]
        public FacebookAttachment[] Attachments { get; set; }
    }
}