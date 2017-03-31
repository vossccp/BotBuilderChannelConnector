using Bot.Builder.ChannelConnector.Directline;
using Microsoft.Bot.Connector;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Owin.DirectLine
{
    public class DirectlineMiddleware : ChannelConnectorMiddleware
    {
        readonly DirectlineConfig[] configs;

        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public DirectlineMiddleware(OwinMiddleware next, DirectlineConfig[] configs, Func<IMessageActivity, Task> onActivityAsync)
            : base(next, onActivityAsync)
        {
            this.configs = configs;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (AuthenticationHeaderValue.TryParse(context.Request.Headers["Authorization"], out AuthenticationHeaderValue value))
            {
                var config = configs.SingleOrDefault(c => c.ApiKey == value.Parameter);
                if (config == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
                if (!context.Request.Uri.LocalPath.StartsWith("directline"))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                if (context.Request.Uri.LocalPath.EndsWith("conversations"))
                {
                    HandleConversationsRequests(config, context);
                }

                if (context.Request.Uri.LocalPath.EndsWith("activities"))
                {
                    await HandleActivitiesRequests(config, context);
                }
            }
            else
            {
                await Next.Invoke(context);
            }            
        }

        static string GetConversationId(Uri uri)
        {
            for (int i = 0; i < uri.Segments.Length; i++)
            {
                var segment = uri.Segments[i];

                if (segment.StartsWith("conversations") && i < uri.Segments.Length - 1)
                {
                    return uri.Segments[i + 1].Substring(0, DirectlineChat.MaxLengthConversationId);
                }
            }
            return null;
        }

        async Task HandleActivitiesRequests(DirectlineConfig config, IOwinContext context)
        {
            var conversationId = GetConversationId(context.Request.Uri);
            if (context.Request.Method == "GET")
            {
                var chat = new DirectlineChat(conversationId, config.ChatLog);
                var watermark = context.Request.Query["watermark"];

                var startIndex = 0;
                if (!string.IsNullOrEmpty(watermark))
                {
                    startIndex = int.Parse(watermark);
                }

                var activities = (await chat.GetActvitiesAsync()).Skip(startIndex).ToList();
                var lastActivity = activities.LastOrDefault();
                if (lastActivity != null)
                {
                    watermark = DirectlineActivityId.Parse(lastActivity.Id).Sequence.ToString();
                }

                var result = new
                {
                    activities = activities,
                    watermark = watermark
                };
               
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result, serializerSettings));
            }
            if (context.Request.Method == "POST")
            {
                var content = await new StreamReader(context.Request.Body).ReadToEndAsync();
                var activity = JsonConvert.DeserializeObject<Activity>(content);

                var chat = new DirectlineChat(conversationId, config.ChatLog);
                var botAccount = new ChannelAccount
                {
                    Id = conversationId,
                    Name = config.BotName
                };

                if(!await chat.IsMemberAsync(botAccount))
                {
                    var memberAddedActivity = await chat.AddMemberAsync(botAccount);
                    await OnMessageReceived(memberAddedActivity);

                    // client might have changed (added messages) to the chat
                    chat.Refresh();                    
                }
                if (!await chat.IsMemberAsync(activity.From))
                {
                    var memberAddedActivity = await chat.AddMemberAsync(activity.From);
                    await OnMessageReceived(memberAddedActivity);

                    // client might have changed (added messages) to the chat
                    chat.Refresh();
                }

                // add received activity to the chat
                await chat.ReceivedAsync(activity);
                
                var result = new
                {
                    id = activity.Id.ToString()
                };

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result, serializerSettings));

                await OnMessageReceived(activity);
            }
        }

        void HandleConversationsRequests(DirectlineConfig config, IOwinContext context)
        {
            if (context.Request.Method == "POST")
            {
                // create a new Conversation

                var chat = new DirectlineChat(config.ChatLog);

                var response = new DirectlineConversation()
                {
                    Id = chat.ConversationId,
                    Token = "ABC",
                    ExpiresIn = 1800
                };

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(response, serializerSettings));

                return;
            }
        }
    }
}
