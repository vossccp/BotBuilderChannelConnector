using Owin;
using System.Configuration;
using Bot.Builder.ChannelConnector.Owin.Facebook;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;

namespace BotBuilderChannelConnector.Demo.AspNet
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RegisterInMemoryBotStore();

            var settings = ConfigurationManager.AppSettings;

            app.UseFacebookMessenger(
                config: new FacebookApiConfig
                {
                    Path = "/messages",                    
                    VerifyToken = settings["VerificationToken"]
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
