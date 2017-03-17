using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot.Builder.ChannelConnector.Facebook
{
    public class FacebookConfig
    {
        public string AppSecret { get; set; }
        public string PageAccessToken { get; set; }
        public string PageId { get; set; }
        public string AppId { get; set; }
    }
}