using Microsoft.Bot.Connector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            chatLogs.TryAdd(conversationId, result);
            return result;
        }

        static readonly ConcurrentDictionary<string, DirectlineChat> chatLogs
            = new ConcurrentDictionary<string, DirectlineChat>();

        public static DirectlineChat Get(string apiKey, string conversationId)
        {
            if (chatLogs.TryGetValue(conversationId, out DirectlineChat result))
            {
                return result;
            }
            return null;
        }

        readonly List<Activity> activities;
        readonly List<ChannelAccount> members;
        readonly DirectlineConfig config;

        DirectlineChat(DirectlineConfig config, string converstaionId)
        {
            this.config = config;
            ConversationId = converstaionId;
            activities = new List<Activity>();
            members = new List<ChannelAccount>();
        }

        public IEnumerable<Activity> Actvities => activities.AsReadOnly();

        void MessageReceived(Activity activity)
        {
            activity.Recipient = new ChannelAccount
            {
                Id = BotName,
                Name = BotName
            };

            activity.Conversation = new ConversationAccount
            {
                Id = ConversationId
            };

            activities.Add(activity);
        }

        public bool IsMember(ChannelAccount account)
        {
            return members.Any(m => m.Id == account.Id);
        }

        public Activity AddMember(ChannelAccount account)
        {
            if (IsMember(account))
            {
                throw new ArgumentException($"{account.Id} already is a member");
            }

            members.Add(account);

            return new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Type = ActivityTypes.ConversationUpdate,
                From = new ChannelAccount
                {
                    Id = ConversationId
                },
                ChannelId = "directline",
                Conversation = new ConversationAccount
                {
                    Id = ConversationId
                },
                Recipient = new ChannelAccount
                {
                    Id = BotName,
                    Name = BotName
                },
                ServiceUrl = string.Empty,
                MembersAdded = new List<ChannelAccount>
                {
                    new ChannelAccount
                    {
                        Id = account.Id,
                        Name = account.Name
                    }
                }
            };
        }

        public void SendMessage(Activity activity)
        {
            activities.Add(activity);
        }

        public Task SendAsync(Activity activity)
        {
            Add(activity);
            return Task.CompletedTask;
        }

        public Task ReceivedAsync(Activity activity)
        {
            Add(activity);
            return Task.CompletedTask;
        }

        void Add(Activity activity)
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
            activity.ServiceUrl = string.Empty;

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
