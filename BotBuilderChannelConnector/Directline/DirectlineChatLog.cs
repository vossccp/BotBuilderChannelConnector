using Microsoft.Bot.Connector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bot.Builder.ChannelConnector.Directline
{
    public class DirectlineChat
    {
        public const int MaxLengthConversationId = 22;
               
        public static DirectlineChat NewConversation(DirectlineConfig config)
        {
            // http://web.archive.org/web/20100408172352/http://prettycode.org/2009/11/12/short-guid/

            var shortGuid = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Substring(0, 22)
                    .Replace("/", "_")
                    .Replace("+", "-");

            return AddConversation(config, shortGuid);
        }

        public static DirectlineChat AddConversation(DirectlineConfig config, string conversationId)
        {
            var result = new DirectlineChat(config, conversationId);
            chatLogs.TryAdd($"{config.ApiKey}|{conversationId}", result);
            return result;
        }

        static readonly ConcurrentDictionary<string, DirectlineChat> chatLogs
            = new ConcurrentDictionary<string, DirectlineChat>();

        public static DirectlineChat Get(string apiKey, string conversationId)
        {
            var key = $"{apiKey}|{conversationId}";
            if (chatLogs.TryGetValue(key, out DirectlineChat result))
            {
                return result;
            }
            return null;
        }

        readonly List<Activity> activities;
        readonly DirectlineConfig config;

        DirectlineChat(DirectlineConfig config, string converstaionId)
        {
            this.config = config;
            ConversationId = converstaionId;
            activities = new List<Activity>();
        }

        public IEnumerable<Activity> Actvities => activities.AsReadOnly();

        void MessageReceived(Activity activity)
        {
            activity.Recipient = new ChannelAccount
            {
                Id = BotId,
                Name = BotName
            };

            activity.Conversation = new ConversationAccount
            {
                Id = ConversationId
            };

            activities.Add(activity);
        }

        public void SendMessage(Activity activity)
        {
            activities.Add(activity);
        }

        public void Add(Activity activity)
        {
            if (activity.Id != null)
            {
                throw new ArgumentException("Activity has an Id assigned and is therefore assumed to belong to another chat");
            }

            if (activity.Conversation == null || activity.Conversation.Id == null)
            {
                activity.Conversation = new ConversationAccount
                {
                    Id = ConversationId
                };
            }
            else if (activity.Conversation.Id != ConversationId)
            {
                throw new ArgumentException("Activity belongs to another conversation");
            }

            activity.Id = new DirectlineActivityId(ConversationId, activities.Count).ToString();
            activity.ChannelId = "directline";
            activity.ServiceUrl = "http://funnyhost.de";

            if (activity.Recipient == null || activity.Recipient.Id == BotId)
            {
                MessageReceived(activity);
            }
            else
            {
                SendMessage(activity);
            }
        }

        public string ConversationId { get; }
        public string BotId => config.ApiKey;
        public string BotName => config.BotName;
    }
}
