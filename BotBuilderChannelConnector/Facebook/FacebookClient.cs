using Bot.Builder.ChannelConnector.Facebook.Schema;
using Microsoft.Bot.Connector;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Facebook
{
    public class FacebookClient
    {
        const string Url = "https://graph.facebook.com/v2.6/me/messages?access_token=";
        const string GraphUrl = "https://graph.facebook.com/v2.8";

        readonly string pageAccessToken;        

        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public FacebookClient(string pageAccessToken)
        {
            if (string.IsNullOrEmpty(pageAccessToken))
            {
                throw new ArgumentNullException(nameof(pageAccessToken));
            }

            this.pageAccessToken = pageAccessToken;            
        }

        public async Task<HttpOperationResponse<object>> SendAsync(Activity activity)
        {
            var messages = activity.ToFacebookMessaging();

            HttpOperationResponse<object> result = null;
            foreach (var message in messages)
            {
                result = await SendAsync(message);
                if (!result.Response.IsSuccessStatusCode)
                {
                    return result;
                }
            }
            return result;
        }

        public async Task<FacebookUserProfile> GetUserProfileAsync(string userId)
        {
            using (var client = new HttpClient())
            {
                var escapedUserId = Uri.EscapeUriString(userId);
                //See: https://developers.facebook.com/docs/messenger-platform/user-profile
                var url = $"{GraphUrl}/{escapedUserId}?fields=first_name,last_name,locale,timezone,gender&access_token={pageAccessToken}";

                var reponse = await client.GetAsync(url);

                if (reponse.IsSuccessStatusCode)
                {
                    var response = await reponse.Content.ReadAsStringAsync();
                    var profile = JsonConvert.DeserializeObject<FacebookUserProfile>(response);
                    profile.Id = profile.Id ?? userId;
                    return profile;
                }

                return null;
            }
        }

        public async Task<HttpOperationResponse<object>> SendAsync(FacebookOutboundMessaging messaging)
        {
            Trace.TraceInformation("Sending message");

            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(messaging, serializerSettings);

                Trace.TraceInformation(content);

                var request = new HttpRequestMessage(HttpMethod.Post, Url + pageAccessToken)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                using (var response = await client.SendAsync(request))
                {
                    var result = new HttpOperationResponse<object>()
                    {
                        Request = request,
                        Response = response
                    };

                    if (response.IsSuccessStatusCode)
                    {
                        var strContent = response.Content.AsString();
                        var fbResponse = JsonConvert.DeserializeObject<FacebookResponse>(strContent);
                        result.Body = new ResourceResponse(fbResponse.MessageId);
                    }

                    return result;
                }
            }
        }

        public async Task<string> Send(string recipient, string message)
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = recipient
                },
                Text = message
            };

            var response = await SendAsync(activity);
            if (!response.Response.IsSuccessStatusCode)
            {
                throw new Exception("Bad request");
            }
            return (response.Body as ResourceResponse).Id;
        }
    }
}
