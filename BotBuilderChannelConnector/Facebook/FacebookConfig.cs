using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook
{
    public class FacebookConfig
    {
        public string Path { get; set; }
        public string VerifyToken { get; set; }
        public string AppSecret { get; set; }
        public string PageAccessToken { get; set; }
        public string PageId { get; set; }
        public string AppId { get; set; }
    }
}