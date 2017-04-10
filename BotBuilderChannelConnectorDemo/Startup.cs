using Microsoft.Bot.Builder.Dialogs;
using Owin;
using System.Threading.Tasks;
using Bot.Builder.ChannelConnector.Facebook;
using System.Configuration;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Bot.Builder.ChannelConnector.Owin.Facebook;
using Bot.Builder.ChannelConnector.Owin.DirectLine;
using Bot.Builder.ChannelConnector.Directline;
using System;
using System.Linq;

namespace Bot.Builder.ChannelConnector.Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            RegisterInMemoryBotStore();

            var settings = ConfigurationManager.AppSettings;

            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            appBuilder.UseDirectline(
                config: new DirectlineConfig
                {
                    BotName = "Testbot",
                    ApiKey = "97IvKio_Vdk.cwA.1hs.GiH9JCCBjWDfVZOyzBnkcYT7yH-Aa_g843YBfCN_tBM",
                    ChatLog = new InMemoryChatLog()
                },
                onActivityAsync: async (activity) =>
                {
                    var a = activity as Activity;
                    switch (a.GetActivityType())
                    {
                        case ActivityTypes.ConversationUpdate:
                            var chat = new DirectlineChat(activity.Conversation.Id, new InMemoryChatLog());
                            var client = new DirectlineConnectorClient(chat);

                            IConversationUpdateActivity update = a;
                            if (update.MembersAdded.Any())
                            {
                                var reply = a.CreateReply();
                                var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                                foreach (var newMember in newMembers)
                                {
                                    reply.Text = "Welcome";
                                    if (!string.IsNullOrEmpty(newMember.Name))
                                    {
                                        reply.Text += $" {newMember.Name}";
                                    }
                                    reply.Text += "!";
                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                            break;
                        default:
                            await Conversation.SendAsync(activity, () => new EchoDialog());
                            break;
                    }
                }
            );

            appBuilder.UseFacebookMessenger(
                config: new FacebookConfig
                {
                    Path = "/messages",
                    PageId = settings["PageId"],
                    AppId = settings["AppId"],
                    AppSecret = settings["AppSecret"],
                    VerifyToken = settings["VerificationToken"],
                    PageAccessToken = settings["PageAccessToken"]
                },
                onActivityAsync: (activity) =>
                {
                    return Conversation.SendAsync(activity, () => new EchoDialog());
                }
            );
        }

        static void RegisterInMemoryBotStore()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<InMemoryDataStore>()
                .AsSelf()
                .SingleInstance();

            builder.Register(c => new CachingBotDataStore(c.Resolve<InMemoryDataStore>(), CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                .As<IBotDataStore<BotData>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}
