using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Bot.Builder.ChannelConnector.Facebook;
using Bot.Builder.ChannelConnector.Facebook.Schema;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Owin.Facebook
{
    public class FacebookMessengerController : ApiController
    {
        static readonly ConcurrentDictionary<string, FacebookUserProfile> profileCache;

        static FacebookMessengerController()
        {
            profileCache = new ConcurrentDictionary<string, FacebookUserProfile>();
        }

        readonly Dictionary<string, FacebookConfig> configs;
        readonly Func<IMessageActivity, Task> onActivityAsync;

        public FacebookMessengerController(Dictionary<string, FacebookConfig> configs, Func<IMessageActivity, Task> onActivityAsync)
        {
            this.configs = configs;
            this.onActivityAsync = onActivityAsync;
        }

        public int Get(
            [FromUri(Name = "hub.mode")] string hubMode,
            [FromUri(Name = "hub.challenge")] string hubChallenge,
            [FromUri(Name = "hub.verify_token")] string hubVerifyToken)
        {
            return int.Parse(hubChallenge);
        }

        public async Task Post(FacebookRequestMessage message)
        {
            var activities = message.ToMessageActivities();

            foreach (var activity in activities)
            {
                Trace.TraceInformation("Recieved activity {0} for {1}", activity.Id, activity.Recipient.Id);
                await TryAddUserProfileAsync(activity);
                await ChannelConnectorOwin.OnMessageReceived(activity, onActivityAsync);
            }
        }

        async Task TryAddUserProfileAsync(IMessageActivity activity)
        {
            var config = configs[activity.Recipient.Id];

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
                    profileCache.TryAdd(userId, user);
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
    }
}
