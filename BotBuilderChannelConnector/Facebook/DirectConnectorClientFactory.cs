using Microsoft.Bot.Builder.Dialogs.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Bot.Builder.ChannelConnector.Facebook;
using Bot.Builder.ChannelConnector.Directline;

namespace Bot.Builder.ChannelConnector
{
    public class FacebookConnectorClientFactory : IConnectorClientFactory
    {
        readonly string pageAccessToken;

        public FacebookConnectorClientFactory(string pageAccessToken)
        {
            this.pageAccessToken = pageAccessToken;
        }

        public IConnectorClient MakeConnectorClient()
        {
            return new FacebookConnectorClient(new FacebookClient(pageAccessToken));
        }

        public IStateClient MakeStateClient()
        {
            throw new NotImplementedException();
        }
    }
}