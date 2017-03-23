using Microsoft.Bot.Connector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class DirectlineChatLog
    {
        public const int MaxLengthConversationId = 22;

        public static DirectlineChatLog NewConversation(string botId)
        {
            // http://web.archive.org/web/20100408172352/http://prettycode.org/2009/11/12/short-guid/

            var shortGuid = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Substring(0, 22)
                    .Replace("/", "_")
                    .Replace("+", "-");

            return new DirectlineChatLog(botId, shortGuid);
        }

        static readonly ConcurrentDictionary<string, DirectlineChatLog> chatLogs
            = new ConcurrentDictionary<string, DirectlineChatLog>();

        public static DirectlineChatLog GetLog(string botId, string conversationId)
        {
            var key = $"{botId}|{conversationId}";
            if (chatLogs.TryGetValue(key, out DirectlineChatLog result))
            {
                return result;
            }
            return new DirectlineChatLog(botId, conversationId);
        }

        readonly List<Activity> activities;

        DirectlineChatLog(string botId, string converstaionId)
        {
            BotId = botId;
            ConversationId = converstaionId;
            activities = new List<Activity>();
        }

        public void Add(Activity activity)
        {
            var activityId = new DirectlineActivityId(ConversationId, 1);
            activity.Id = activityId.ToString();
            activity.Conversation = new ConversationAccount
            {
                Id = ConversationId
            };
        }

        public string ConversationId { get; }
        public string BotId { get; }
    }
}
