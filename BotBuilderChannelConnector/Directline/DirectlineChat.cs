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

        List<Activity> activities;
        readonly List<ChannelAccount> members;
        readonly DirectlineConfig config;
        readonly IChatLog chatLog;


        DirectlineChat(DirectlineConfig config, string converstaionId)
        {
            this.config = config;
            ConversationId = converstaionId;
            members = new List<ChannelAccount>();
            chatLog = new InMemoryChatLog();
        }

        async Task EnsureLoaded()
        {
            if (activities != null) return;

            var log = await chatLog.GetActivitiesAsync(ConversationId);
            activities = new List<Activity>();

            foreach (var item in log)
            {
                if (item.GetActivityType() == ActivityTypes.ConversationUpdate)
                {
                    members.AddRange(item.MembersAdded);
                }
                else
                {
                    activities.Add(item);
                }
            }
        }

        public async Task<IEnumerable<Activity>> GetActvitiesAsync()
        {
            await EnsureLoaded();
            return activities.AsReadOnly();
        }

        async Task MessageReceivedAsync(Activity activity)
        {
            await EnsureLoaded();

            activity.Recipient = new ChannelAccount
            {
                Id = BotName,
                Name = BotName
            };

            activity.Conversation = new ConversationAccount
            {
                Id = ConversationId
            };

            await chatLog.StoreAsync(activity);
            activities.Add(activity);
        }

        public bool IsMember(ChannelAccount account)
        {
            return members.Any(m => m.Id == account.Id);
        }

        public async Task<Activity> AddMemberAsync(ChannelAccount account)
        {
            await EnsureLoaded();

            if (IsMember(account))
            {
                throw new ArgumentException($"{account.Id} already is a member");
            }

            members.Add(account);

            var result = new Activity
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

            await chatLog.StoreAsync(result);

            return result;
        }

        public async Task SendActivityAsync(Activity activity)
        {
            await AddAsync(activity);
        }

        public async Task ReceivedAsync(Activity activity)
        {
            activity.Recipient = new ChannelAccount
            {
                Id = BotName,
                Name = BotName
            };

            await AddAsync(activity);
        }

        async Task AddAsync(Activity activity)
        {
            if (activity.Id != null)
            {
                throw new ArgumentException("Activity has an Id assigned and is therefore assumed to belong to another chat");
            }

            await EnsureLoaded();

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

            await chatLog.StoreAsync(activity);
            activities.Add(activity);
        }

        public string ConversationId { get; }
        public string BotId => config.ApiKey;
        public string BotName => config.BotName;
    }
}
