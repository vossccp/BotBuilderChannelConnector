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

        List<Activity> activities;
        readonly List<ChannelAccount> members;
        readonly IChatLog chatLog;

        string name;

        static string NewConversationId()
        {
            // http://web.archive.org/web/20100408172352/http://prettycode.org/2009/11/12/short-guid/

            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Substring(0, 22)
                    .Replace("/", "_")
                    .Replace("+", "-");
        }


        public DirectlineChat(string converstaionId, IChatLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            ConversationId = converstaionId;
            members = new List<ChannelAccount>();
            chatLog = log;
            name = ConversationId;
        }

        public DirectlineChat(IChatLog log)
             : this(NewConversationId(), log)
        {            
        }

        async Task EnsureLoadedAsync()
        {
            if (activities != null) return;

            var log = await chatLog.GetActivitiesAsync(ConversationId);
            activities = new List<Activity>();

            foreach (var item in log)
            {
                if (item.GetActivityType() == ActivityTypes.ConversationUpdate)
                {
                    foreach (var member in item.MembersAdded)
                    {
                        ApplyMember(member);
                    }
                }
                else
                {
                    activities.Add(item);
                }
            }
        }

        public void Refresh()
        {
            activities = null;
        }

        public async Task<IEnumerable<Activity>> GetActvitiesAsync()
        {
            await EnsureLoadedAsync();
            return activities.Where(a => a.GetActivityType() == ActivityTypes.Message);
        }

        async Task MessageReceivedAsync(Activity activity)
        {
            await EnsureLoadedAsync();

            activity.Recipient = new ChannelAccount
            {
                Id = ConversationId,
                Name = Name
            };

            activity.Conversation = new ConversationAccount
            {
                Id = ConversationId
            };

            await chatLog.StoreAsync(activity);
            activities.Add(activity);
        }

        void ApplyMember(ChannelAccount account)
        {
            members.Add(account);

            if (account.Id == ConversationId)
            {
                name = account.Name ?? ConversationId;
            }
        }

        public async Task<bool> IsMemberAsync(ChannelAccount account)
        {
            await EnsureLoadedAsync();
            return members.Any(m => m.Id == account.Id);
        }

        public async Task<Activity> AddMemberAsync(ChannelAccount account)
        {
            if (await IsMemberAsync(account))
            {
                throw new ArgumentException($"{account.Id} already is a member");
            }

            ApplyMember(account);

            var result = new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Type = ActivityTypes.ConversationUpdate,
                From = new ChannelAccount
                {
                    Id = ConversationId,
                    Name = Name
                },
                ChannelId = "directline",
                Conversation = new ConversationAccount
                {
                    Id = ConversationId
                },
                Recipient = new ChannelAccount
                {
                    Id = ConversationId,
                    Name = Name
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
                Id = ConversationId,
                Name = Name
            };

            await AddAsync(activity);
        }

        async Task AddAsync(Activity activity)
        {
            if (activity.Id != null)
            {
                throw new ArgumentException("Activity has an Id assigned and is therefore assumed to belong to another chat");
            }

            await EnsureLoadedAsync();

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
        public string Name => name;
    }
}
