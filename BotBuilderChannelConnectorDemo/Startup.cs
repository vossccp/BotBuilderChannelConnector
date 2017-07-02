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

	        var chatLog = new InMemoryChatLog();
	        const string botName = "Testbot";

            appBuilder.UseDirectline(
                config: new DirectlineConfig
                {
                    BotName = botName,
                    ApiKey = "97IvKio_Vdk.cwA.1hs.GiH9JCCBjWDfVZOyzBnkcYT7yH-Aa_g843YBfCN_tBM",
                    ChatLog = chatLog
                },
                onActivityAsync: async activity =>
                {
                    var a = activity as Activity;
                    switch (a.GetActivityType())
                    {
                        case ActivityTypes.ConversationUpdate:
	                        var chat = new DirectlineChat(activity.Conversation.Id, chatLog);
							var client = new DirectlineConnectorClient(chat);							

                            IConversationUpdateActivity update = a;
                            if (update.MembersAdded.Any())
                            {
                                var message = await chat.CreateMessageAsync();

                                if (update.MembersAdded.Any(t => t.Id == botName))
                                {
                                    // Bot added as Channel Member, can be used as a welcome message
									// when the chat starts
                                    message.Text = "Welcome to our chat";
                                    await client.Conversations.ReplyToActivityAsync(message);
                                }
                                else
                                {
									// display a welcome message for all members
                                    var newMembers = update.MembersAdded.Where(t => t.Id != botName);
                                    foreach (var newMember in newMembers)
                                    {
                                        message.Text = "Welcome";
                                        if (!string.IsNullOrEmpty(newMember.Name))
                                        {
                                            message.Text += $" {newMember.Name}";
                                        }
                                        message.Text += "!";
                                        await client.Conversations.ReplyToActivityAsync(message);
                                    }
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
