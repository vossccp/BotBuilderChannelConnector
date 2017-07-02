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
        readonly ConcurrentDictionary<string, List<Activity>> activityCache;
	    readonly HashSet<IChatLogListener> chatLogListeners;

        public InMemoryChatLog()
        {
            activityCache = new ConcurrentDictionary<string, List<Activity>>();
			chatLogListeners = new HashSet<IChatLogListener>();
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
			NotifyListeners(activity);
            return Task.CompletedTask;
        }

	    void NotifyListeners(Activity activity)
	    {
		    foreach (var listener in chatLogListeners)
		    {
			    listener.OnActivity(activity);
		    }
		}

		public void AddListener(IChatLogListener listener)
	    {
		    if (chatLogListeners.Contains(listener))
		    {
			    throw new InvalidOperationException("Listener already exists");
		    }
		    chatLogListeners.Add(listener);
	    }

	    public void RemoveListener(IChatLogListener listener)
	    {
		    chatLogListeners.Remove(listener);
	    }
    }
}
