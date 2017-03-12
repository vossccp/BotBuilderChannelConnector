using Microsoft.Bot.Builder.Dialogs;
using Owin;
using System.Threading.Tasks;
using Vossccp.BotBuilder.ChannelConnector.Facebook;
using System.Configuration;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Vossccp.BotBuilder.ChannelConnector.Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            RegisterInMemoryBotStore();

            var settings = ConfigurationManager.AppSettings;

            appBuilder.UseFacebookMessenger(
                config: new FacebookConfig
                {
                    PageId = settings["PageId"],
                    AppId = settings["AppId"],
                    AppSecret = settings["AppSecret"],
                    PageAccessToken = settings["PageAccessToken"]
                },
                onActivityAsync: (activiy) =>
                {
                    Conversation.SendAsync(activiy, () => new DemoDialog());
                    return Task.CompletedTask;
                }
            );
        }

        static void RegisterInMemoryBotStore()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<InMemoryDataStore>()
                .AsSelf()
                .SingleInstance();

            builder
                .Register(c => new DirectConnectorClientFactory(c.Resolve<FacebookConfig>()))
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder.Register(c => new CachingBotDataStore(c.Resolve<InMemoryDataStore>(), CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                .As<IBotDataStore<BotData>>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.Update(Conversation.Container);
        }
    }
}
