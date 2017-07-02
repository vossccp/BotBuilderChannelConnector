using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Directline
{
    public interface IChatLog
    {
        Task<IEnumerable<Activity>> GetActivitiesAsync(string conversationId);
        Task StoreAsync(Activity activity);
	    void AddListener(IChatLogListener listener);
	    void RemoveListener(IChatLogListener listener);
    }
}
