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
    public class DirectConnectorClientFactory : IConnectorClientFactory
    {
        readonly IMessageActivity activity;

        public DirectConnectorClientFactory(IMessageActivity activity)
        {
            this.activity = activity;
        }

        public IConnectorClient MakeConnectorClient()
        {
            var channel = activity.ChannelId;
            if (channel == "facebook")
            {
                var fbConfig = FacebookMessenger.GetConfig(activity.Recipient.Id);
                return new FacebookConnectorClient(new FacebookClient(fbConfig.PageAccessToken));
            }

            if (channel == "directline")
            {
                var chat = DirectlineChat.Get(activity.Recipient.Id, activity.Conversation.Id);
                return new DirectlineConnectorClient(chat);
            }

            return null;
        }

        public IStateClient MakeStateClient()
        {
            throw new NotImplementedException();
        }
    }
}