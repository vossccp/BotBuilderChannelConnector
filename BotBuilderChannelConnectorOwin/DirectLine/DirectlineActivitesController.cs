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
    public class DirectlineActivitesController : ApiController
    {
        public DirectlineConfig DirectlineConfig
            => configs[User.Identity.Name];

        readonly Dictionary<string, DirectlineConfig> configs;
        readonly Func<IMessageActivity, Task> onActivityAsync;

        public DirectlineActivitesController(Dictionary<string, DirectlineConfig> configs, Func<IMessageActivity, Task> onActivityAsync)
        {
            this.configs = configs;
            this.onActivityAsync = onActivityAsync;
        }

		public async Task<dynamic> Get(string id, int? watermark)
        {
            var skip = watermark.GetValueOrDefault(0);
	        var chat = DirectlineChat.Get(id);

            var activities = (await chat.GetActvitiesAsync()).Skip(skip).ToList();

            return new
            {
                Activities = activities,
                Watermark = activities.Count
            };
        }

		public async Task<dynamic> Post(string id, Activity activity)
		{
			var chat = DirectlineChat.Get(id);

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
