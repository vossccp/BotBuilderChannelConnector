using Autofac;
using Bot.Builder.ChannelConnector.Directline;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Newtonsoft.Json.Serialization;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    public static class DirectLineOwinConfig
    {
        public static IAppBuilder UseDirectline(this IAppBuilder appBuilder, DirectlineConfig config, Func<IMessageActivity, Task> onActivityAsync)
        {
            return appBuilder.UseDirectline(new[] { config }, onActivityAsync);
        }

        public static IAppBuilder UseDirectline(this IAppBuilder appBuilder, DirectlineConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
        {
            ChannelConnector.AddDirectlineConfig(configs);

            var httpConfig = new HttpConfiguration();

            var builder = new ContainerBuilder();

            builder.RegisterType<DirectlineConversationsController>().InstancePerRequest();
            builder.RegisterType<DirectlineActivitesController>().InstancePerRequest();

            builder.RegisterInstance(configs.ToDictionary(c => c.ApiKey, c => c)).SingleInstance();
            builder.RegisterInstance(onActivityAsync);

            builder
                .Register(c => new DirectlineAuthorizationFilter(c.Resolve<Dictionary<string, DirectlineConfig>>()))
                .AsWebApiAuthorizationFilterFor<DirectlineConversationsController>()
                .InstancePerRequest();

	        builder
		        .Register(c => new DirectlineAuthorizationFilter(c.Resolve<Dictionary<string, DirectlineConfig>>()))				
		        .AsWebApiAuthorizationFilterFor<DirectlineActivitesController>()
		        .InstancePerRequest();

			builder.RegisterWebApiFilterProvider(httpConfig);

            var container = builder.Build();
            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var formatter = httpConfig.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            httpConfig.Routes.MapHttpRoute
            (
                name: "directline",
                routeTemplate: "directline/conversations/{id}",
                defaults: new
                {
                    controller = "DirectlineConversations",
                    id = RouteParameter.Optional
                }
            );

	        httpConfig.Routes.MapHttpRoute
	        (
		        name: "directline-activities",
		        routeTemplate: "directline/conversations/{id}/activities",
		        defaults: new
		        {
			        controller = "DirectlineActivites",
			        id = RouteParameter.Optional
		        }
	        );

			appBuilder.UseAutofacWebApi(httpConfig);
            return appBuilder.UseWebApi(httpConfig);
        }
    }
}
