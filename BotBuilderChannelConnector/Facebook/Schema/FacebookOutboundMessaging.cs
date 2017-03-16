using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotBuilder.ChannelConnector.Facebook.Schema
{
    public class FacebookOutboundMessaging
    {
        [JsonProperty("recipient")]
        public FacebookRecipient Recipient { get; set; }
        [JsonProperty("sender_action")]
        public string SenderAction { get; set; }
        [JsonProperty("message")]
        public FacebookOutboundMessage Message { get; set; }
    }
}
