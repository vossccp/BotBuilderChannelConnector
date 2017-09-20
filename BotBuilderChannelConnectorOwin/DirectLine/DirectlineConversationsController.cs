using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Bot.Builder.ChannelConnector.Directline;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
	public class DirectlineConversationsController : ApiController
	{
		public DirectlineConfig DirectlineConfig
			=> configs[User.Identity.Name];

		readonly Dictionary<string, DirectlineConfig> configs;
		readonly Func<IMessageActivity, Task> onActivityAsync;
		readonly DirectlineWebSocketHandlerRegistry handlerRegistry;

		public DirectlineConversationsController(Dictionary<string, DirectlineConfig> configs, Func<IMessageActivity, Task> onActivityAsync, DirectlineWebSocketHandlerRegistry handlerRegistry)
		{
			this.configs = configs;
			this.onActivityAsync = onActivityAsync;
			this.handlerRegistry = handlerRegistry;
		}

		public async Task<DirectlineConversation> Post()
		{
			var newConversationId = DirectlineChat.NewConversationId();

			var chat = new DirectlineChat(newConversationId, DirectlineConfig.ChatLog);

			await InitChatAsync(chat);

			return CreateConversationResponse(chat.ConversationId);
		}

		DirectlineConversation CreateConversationResponse(string conversationId)
		{
			var token = User.Identity.Name; // maps to directline secret
			string wsUri = null;

			if (handlerRegistry.IsWebSocketSupported)
			{
				var uri = Request.RequestUri;
				var handler = handlerRegistry.Create(conversationId, DirectlineConfig.ChatLog);
				wsUri = $"ws://{uri.Host}:{uri.Port}/directline/conversations/{conversationId}/stream?t={HttpUtility.UrlEncode(handler.Token)}";
				token = handler.Token;
			}

			return new DirectlineConversation
			{
				Id = conversationId,
				Token = token,
				ExpiresIn = (int)DirectlineWebSocketHandler.TokenExpirationTime.TotalSeconds,
				StreamUrl = wsUri
			};
		}

		async Task InitChatAsync(DirectlineChat chat)
		{
			var botAccount = new ChannelAccount
			{
				Id = DirectlineConfig.BotName,
				Name = DirectlineConfig.BotName
			};

			if (!await chat.IsMemberAsync(botAccount))
			{
				var memberAddedActivity = await chat.AddMemberAsync(botAccount);
				await ChannelConnectorOwin.OnMessageReceived(memberAddedActivity, onActivityAsync);
			}
		}

		public async Task<DirectlineConversation> Get(string id, int? watermark)
		{
			var chat = new DirectlineChat(id, DirectlineConfig.ChatLog);

			var activities = await chat.GetActvitiesAsync();
			if (!activities.Any())
			{
				await InitChatAsync(chat);
			}

			return CreateConversationResponse(chat.ConversationId);
		}
	}
}
