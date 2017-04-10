using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Bot.Builder.ChannelConnector.Directline;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    [DirectlineAuthorize]
    public class ConversationsController : ApiController
    {
        public DirectlineConfig DirectlineConfig
            => Configuration.Properties[User.Identity.Name] as DirectlineConfig;

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
                await OnMessageReceived(memberAddedActivity);

                // client might have changed (added messages) to the chat
                chat.Refresh();
            }
            if (!await chat.IsMemberAsync(activity.From))
            {
                var memberAddedActivity = await chat.AddMemberAsync(activity.From);
                await OnMessageReceived(memberAddedActivity);

                // client might have changed (added messages) to the chat
                chat.Refresh();
            }

            // add received activity to the chat
            await chat.ReceivedAsync(activity);
            await OnMessageReceived(activity);

            return new
            {
                activity.Id
            };
        }

        protected async Task OnMessageReceived(IMessageActivity activity)
        {
            var onActivityAsync = Configuration.Properties["callback"] as Func<IMessageActivity, Task>;

            Trace.TraceInformation("Recieved activity {0} for {1}", activity.Id, activity.Recipient.Id);
            try
            {
                await onActivityAsync(activity);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);

                if (exception.InnerException != null)
                {
                    Trace.WriteLine(exception.InnerException.Message);
                }

                throw;
            }
        }
    }
}
