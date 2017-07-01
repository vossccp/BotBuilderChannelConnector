using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class DirectlineConnectorClientFactory : IConnectorClientFactory
    {
        readonly string conversationId;

        public DirectlineConnectorClientFactory(string conversationId)
        {
            this.conversationId = conversationId;
        }

        public IConnectorClient MakeConnectorClient()
        {
            return new DirectlineConnectorClient(DirectlineChat.Get(conversationId));
        }

        public IStateClient MakeStateClient()
        {
            throw new NotImplementedException();
        }
    }
}
