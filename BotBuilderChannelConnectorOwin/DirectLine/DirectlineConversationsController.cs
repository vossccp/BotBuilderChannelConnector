using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

		public DirectlineConversationsController(Dictionary<string, DirectlineConfig> configs, Func<IMessageActivity, Task> onActivityAsync)
		{
			this.configs = configs;
			this.onActivityAsync = onActivityAsync;
		}

		public async Task<DirectlineConversation> Post()
		{
			var chat = DirectlineChat.Create(DirectlineConfig.BotName, DirectlineConfig.ChatLog);

			await InitChatAsync(chat);

			return new DirectlineConversation
			{
				Id = chat.ConversationId,
				Token = "ABC",
				ExpiresIn = 1800
			};
		}

		async Task InitChatAsync(DirectlineChat chat)
		{
			var botAccount = chat.Bot;

			if (!await chat.IsMemberAsync(botAccount))
			{
				var memberAddedActivity = await chat.AddMemberAsync(botAccount);
				await ChannelConnectorOwin.OnMessageReceived(memberAddedActivity, onActivityAsync);
			}
		}

		public async Task<DirectlineConversation> Get(string id, int? watermark)
		{
			var chat = new DirectlineChat(DirectlineConfig.BotName, id, DirectlineConfig.ChatLog);

			var activities = await chat.GetActvitiesAsync();
			if (!activities.Any())
			{
				await InitChatAsync(chat);
			}

			return new DirectlineConversation
			{
				Id = chat.ConversationId,
				Token = "ABC",
				ExpiresIn = 1800
			};
		}
	}
}
