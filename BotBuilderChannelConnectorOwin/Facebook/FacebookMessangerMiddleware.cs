using Microsoft.Bot.Connector;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Bot.Builder.ChannelConnector.Facebook;
using Bot.Builder.ChannelConnector.Facebook.Schema;
using System.Collections.Generic;

namespace Bot.Builder.ChannelConnector.Owin.Facebook
{
    public class FacebookMessangerMiddleware : OwinMiddleware
    {
        readonly FacebookApiConfig config;
        readonly Func<IMessageActivity, Task> onActivityAsync;
        readonly string path;
        readonly Dictionary<string, FacebookUserProfile> profileCache;

        public FacebookMessangerMiddleware(OwinMiddleware next, FacebookApiConfig config, Func<IMessageActivity, Task> onActivityAsync)
            : base(next)
        {
            path = config.Path ?? "/";
            this.config = config;
            this.onActivityAsync = onActivityAsync;
            profileCache = new Dictionary<string, FacebookUserProfile>();
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Uri.LocalPath.Equals(path, StringComparison.InvariantCultureIgnoreCase))
            {
                if (context.Request.Query["hub.mode"] == "subscribe")
                {
                    await Subscribe(context);
                    return;
                }

                if (context.Request.Method == "POST")
                {
                    try
                    {
                        await MessageReceived(context);
                        return;
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(exception.Message);
                        Trace.WriteLine(exception.StackTrace);

                        if (exception.InnerException != null)
                        {
                            Trace.WriteLine(exception.InnerException.Message);
                        }

                        throw;
                    }
                }
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        async Task MessageReceived(IOwinContext context)
        {
            var content = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var message = JsonConvert.DeserializeObject<FacebookRequestMessage>(content);
            var activities = message.ToMessageActivities();

            foreach (var activity in activities)
            {
                Trace.TraceInformation("Recieved activity {0} for {1}", activity.Id, activity.Recipient.Id);
                await TryAddUserProfileAsync(activity);
                await onActivityAsync(activity);
            }
        }

        async Task TryAddUserProfileAsync(IMessageActivity activity)
        {
            if (string.IsNullOrEmpty(config.PageAccessToken))
            {
                return;
            }

            var userId = activity.From.Id;
            FacebookUserProfile user;
            if (!profileCache.TryGetValue(activity.From.Id, out user))
            {
                Trace.TraceInformation("UserProfile for {0} not found in cache", userId);

                var client = new FacebookClient(config.PageAccessToken);
                user = await client.GetUserProfileAsync(userId);
                if (user != null)
                {
                    profileCache.Add(userId, user);
                }
            }

            if (user != null)
            {
                activity.From.Name = $"{user.FirstName} {user.LastName}";
                activity.Locale = user.Locale;
                Trace.TraceInformation("Extended activity with user name {0} and locale {1}", activity.From.Name, activity.Locale);
            }

            // else, throw an Exception?
        }

        async Task Subscribe(IOwinContext context)
        {
            Trace.TraceInformation("Received subscribtion request");

            var verifyToken = context.Request.Query["hub.verify_token"];
            if (Equals(config.VerifyToken, verifyToken))
            {
                await context.Response.WriteAsync(context.Request.Query["hub.challenge"]);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
