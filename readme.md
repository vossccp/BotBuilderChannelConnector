BotBuilderChannelConnector
==========================

Allows to diretly interface channels without using the Microsoft Bot Development portal.

**Bare in mind: *This is work in progress***

When using the Microsoft Bot Framework you are foreced to connect your bot with your target channels through the Microsoft Bot platform http://dev.botframework.com.

The purpose of the Microsoft platform is to transfer channel specific messsages into a common format: the Activity. Additionally, it persists your bot state (dialog as well as user state). In essense it means that all messeges you send or receive are going through a Microsoft server. This is not only critical from a security standpoint, it also results in extra roundtrips and latency which is not justfied by the functionality the portal provides. In order to bypas the entire Microsoft platform, you can use the BotBuilderChannelConnector to interface directly with the Channels you wish to support.

This greatly simplfies the configuration effort as well as it drastically improve the overall performance.

The ChannelConnector is designed as a Owin Middleware and can be setup (for Facebook Messenger) like this


```c#
appBuilder.UseFacebookMessenger(
	config: new FacebookConfig
	{
		Path = "/messages",
		PageId = "your facebook page id",
		AppId = "your facebook app id",
		AppSecret = "your facebook app secret",
		VerifyToken = "your facebook verify token when setting up the web hook",
		PageAccessToken = "your page access token"
	},
	onActivityAsync: (activity) =>
	{
		// your bot code goes here
	}
);
```

As of now, only Facebook Messenger is supported. You are, however, invited to extend the Connector with other Channels you wish to support.

Be aware that you have to provide your own bot state management (which is a good idea in any case). There are examples of storing your data in your own Azure table storage https://github.com/Microsoft/BotBuilder-Azure or in a Redis Store: https://github.com/ankitbko/Microsoft.Bot.Builder.RedisStore.

The easiest way to start is to use the in memory storage provided by the Bot Framework:

```c#
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
```
