using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bot.Builder.ChannelConnector.Facebook
{
    // response from Facebook upon successfull submit of a message
    public class FacebookResponse
    {
        [JsonProperty("recipient_id")]
        public string RecipientId { get; set; }
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
    }
}
