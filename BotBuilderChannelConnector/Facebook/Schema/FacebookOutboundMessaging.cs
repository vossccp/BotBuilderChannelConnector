﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookOutboundMessaging
    {
        [JsonProperty("recipient")]
        public FacebookAccount Recipient { get; set; }
        [JsonProperty("sender_action")]
        public string SenderAction { get; set; }
        [JsonProperty("message")]
        public FacebookOutboundMessage Message { get; set; }
    }
}
