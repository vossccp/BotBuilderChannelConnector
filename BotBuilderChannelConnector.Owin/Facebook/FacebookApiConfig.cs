using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotBuilder.ChannelConnector.Facebook;

namespace BotBuilder.ChannelConnector.Owin.Facebook
{
    public class FacebookApiConfig : FacebookConfig
    {
        public string Path { get; set; }
        public string VerifyToken { get; set; }
    }
}
