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

        public DirectlineConversation Post()
        {
            var chat = new DirectlineChat(DirectlineConfig.ChatLog);
            return new DirectlineConversation
            {
                Id = chat.ConversationId,
                Token = "ABC",
                ExpiresIn = 1800
            };
        }

        [HttpGet]
        public async Task<dynamic> Activities(string id, int? watermark)
        {
            var skip = watermark.GetValueOrDefault(0);
            var chat = new DirectlineChat(id, DirectlineConfig.ChatLog);

            var activities = (await chat.GetActvitiesAsync()).Skip(skip).ToList();
            var lastActivity = activities.LastOrDefault();
            if (lastActivity != null)
            {
                skip = DirectlineActivityId.Parse(lastActivity.Id).Sequence;
            }

            return new
            {
                Activities = activities,
                Watermark = skip
            };
        }

        [HttpPost]
        public async Task<dynamic> Activities(string id, Activity activity)
        {
            var chat = new DirectlineChat(id, DirectlineConfig.ChatLog);
            var botAccount = new ChannelAccount
            {
                Id = id,
                Name = DirectlineConfig.BotName
            };

            if (!await chat.IsMemberAsync(botAccount))
            {
                var memberAddedActivity = await chat.AddMemberAsync(botAccount);
                await ChannelConnectorOwin.OnMessageReceived(memberAddedActivity, onActivityAsync);

                // client might have changed (added messages) to the chat
                chat.Refresh();
            }
            if (!await chat.IsMemberAsync(activity.From))
            {
                var memberAddedActivity = await chat.AddMemberAsync(activity.From);
                await ChannelConnectorOwin.OnMessageReceived(memberAddedActivity, onActivityAsync);

                // client might have changed (added messages) to the chat
                chat.Refresh();
            }

            // add received activity to the chat
            await chat.ReceivedAsync(activity);
            await ChannelConnectorOwin.OnMessageReceived(activity, onActivityAsync);

            return new
            {
                activity.Id
            };
        }
    }
}
