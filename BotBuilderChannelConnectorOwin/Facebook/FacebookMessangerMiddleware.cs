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
using System.Linq;

namespace Bot.Builder.ChannelConnector.Owin.Facebook
{
    public class FacebookMessangerMiddleware : ChannelConnectorMiddleware
    {
        readonly FacebookConfig[] configs;
        readonly Dictionary<string, FacebookUserProfile> profileCache;

        public FacebookMessangerMiddleware(OwinMiddleware next, FacebookConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
            : base(next, onActivityAsync)
        {
            if (configs == null && !configs.Any())
            {
                throw new ArgumentException("No configuration provided");
            }
            if (configs.Any(c => string.IsNullOrEmpty(c.Path)))
            {
                throw new ArgumentException("A valid path configuration has to be provided");
            }
            if (configs.Any(c => string.IsNullOrEmpty(c.VerifyToken)))
            {
                throw new ArgumentException($"A verification token has to be provided");
            }

            this.configs = configs;
            profileCache = new Dictionary<string, FacebookUserProfile>();
        }

        public override async Task Invoke(IOwinContext context)
        {
            var pathExists = configs.Any(c => context.Request.Uri.LocalPath.Equals(c.Path));
            if (pathExists)
            {
                if (context.Request.Query["hub.mode"] == "subscribe")
                {
                    await Subscribe(context);
                    return;
                }

                if (context.Request.Method == "POST")
                {
                    await MessageReceived(context);
                    return;
                }

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        FacebookConfig GetConfig(string pageId)
        {
            return configs.Single(c => c.PageId == pageId);
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
                await OnMessageReceived(activity);
            }
        }

        async Task TryAddUserProfileAsync(IMessageActivity activity)
        {
            var config = GetConfig(activity.Recipient.Id);
            if (config != null && string.IsNullOrEmpty(config.PageAccessToken))
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
            if (configs.Any(c => Equals(c.VerifyToken, verifyToken)))
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
