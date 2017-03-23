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
                    Path = "/directline",
                    ApiKey = "97IvKio_Vdk.cwA.1hs.GiH9JCCBjWDfVZOyzBnkcYT7yH-Aa_g843YBfCN_tBM"
                },
                onActivityAsync: (activity) =>
                {
                    return Conversation.SendAsync(activity, () => new EchoDialog());
                }
            );

            //appBuilder.UseFacebookMessenger(
            //    config: new FacebookConfig
            //    {
            //        Path = "/messages",
            //        PageId = settings["PageId"],
            //        AppId = settings["AppId"],
            //        AppSecret = settings["AppSecret"],
            //        VerifyToken = settings["VerificationToken"],
            //        PageAccessToken = settings["PageAccessToken"]
            //    },
            //    onActivityAsync: (activity) =>
            //    {
            //        return Conversation.SendAsync(activity, () => new DemoDialog());
            //    }
            //);
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
