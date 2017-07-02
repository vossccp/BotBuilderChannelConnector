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
		readonly DirectlineChat chat;

		public DirectlineConnectorClientFactory(DirectlineChat chat)
		{
			this.chat = chat;
		}

		public IConnectorClient MakeConnectorClient()
		{
			return new DirectlineConnectorClient(chat);
		}

		public IStateClient MakeStateClient()
		{
			throw new NotImplementedException();
		}
	}
}
