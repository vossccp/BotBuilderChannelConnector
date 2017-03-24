using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class InMemoryChatLog : IChatLog
    {
        readonly Dictionary<string, List<Activity>> activityCache;

        public InMemoryChatLog()
        {
            activityCache = new Dictionary<string, List<Activity>>();
        }

        List<Activity> GetActivities(string conversationId)
        {
            List<Activity> result;
            if (!activityCache.TryGetValue(conversationId, out result))
            {
                result = new List<Activity>();
            }
            return result;
        }

        public Task<IEnumerable<Activity>> GetActivitiesAsync(string conversationId)
        {
            return Task.FromResult(GetActivities(conversationId).AsReadOnly().AsEnumerable());
        }

        public Task StoreAsync(Activity activity)
        {
            var activities = GetActivities(activity.Conversation.Id);
            activities.Add(activity);
            return Task.CompletedTask;
        }
    }
}
