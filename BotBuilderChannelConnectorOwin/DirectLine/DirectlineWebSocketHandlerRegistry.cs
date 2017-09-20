using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Builder.ChannelConnector.Directline;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	public class DirectlineWebSocketHandlerRegistry
	{
		readonly ConcurrentDictionary<string, DirectlineWebSocketHandler> handlersByToken
			= new ConcurrentDictionary<string, DirectlineWebSocketHandler>();

		public bool IsWebSocketSupported { get; } = true;

		public DirectlineWebSocketHandler GetByToken(string token)
		{
			// Token to be used only one time to establish connection

			if (handlersByToken.TryRemove(token, out DirectlineWebSocketHandler handler))
			{
				if (!handler.IsTokeExpired)
				{
					return handler;
				}
			}
			return null;
		}

		public DirectlineWebSocketHandler Create(string conversationId, IChatLog chatLog)
		{
			var handler = new DirectlineWebSocketHandler(conversationId, chatLog);

			handlersByToken.TryAdd(handler.Token, handler);

			ExpireTokens();

			return handler;
		}

		void ExpireTokens()
		{
			var expiredHandlers = handlersByToken.Values.Where(h => h.IsTokeExpired).ToList();
			foreach (var expiredHandler in expiredHandlers)
			{
				handlersByToken.TryRemove(expiredHandler.Token, out DirectlineWebSocketHandler ignore);
			}
		}
	}
}
