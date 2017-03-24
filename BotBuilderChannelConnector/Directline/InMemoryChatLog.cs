using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Collections.Concurrent;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class InMemoryChatLog : IChatLog
    {
        static readonly ConcurrentDictionary<string, List<Activity>> activityCache;

        static InMemoryChatLog()
        {
            activityCache = new ConcurrentDictionary<string, List<Activity>>();
        }

        List<Activity> GetActivities(string conversationId)
        {
            List<Activity> result;
            if (!activityCache.TryGetValue(conversationId, out result))
            {
                result = new List<Activity>();
                activityCache.TryAdd(conversationId, result);
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
