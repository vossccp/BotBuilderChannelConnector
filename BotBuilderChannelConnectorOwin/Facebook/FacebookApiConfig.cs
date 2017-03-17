using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Builder.ChannelConnector.Facebook;

namespace Bot.Builder.ChannelConnector.Owin.Facebook
{
    public class FacebookApiConfig : FacebookConfig
    {
        public string Path { get; set; }
        public string VerifyToken { get; set; }
    }
}
