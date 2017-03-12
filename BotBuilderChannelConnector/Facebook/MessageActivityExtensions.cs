using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Vossccp.BotBuilder.ChannelConnector.Facebook.Schema;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook
{
    internal static class MessageActivityExtensions
    {
        internal static IEnumerable<IMessageActivity> ToMessageActivities(this FacebookRequestMessage fbRequestMessage)
        {
            return fbRequestMessage.Entries
                .SelectMany(e => e.Messaging)
                .Select(m => new Activity
                {
                    Type = "message",
                    Id = m.Message.Mid,
                    Timestamp = DateTime.UtcNow,
                    ServiceUrl = "https://facebook.botframework.com",
                    ChannelId = "facebook",
                    From = new ChannelAccount
                    {
                        Id = m.Sender.Id
                    },
                    Conversation = new ConversationAccount
                    {
                        IsGroup = false,
                        Id = $"{m.Sender.Id}-{m.Recipient.Id}"
                    },
                    Recipient = new ChannelAccount
                    {
                        Id = m.Recipient.Id
                    },
                    ChannelData = JsonConvert.SerializeObject(m),

                    Text = m?.Message?.QuickReply != null
                            ? m.Message.QuickReply.Payload
                            : m.Message.Text,

                    Attachments = m.Message.Attachments?
                            .Select(a => new Attachment
                            {
                                ContentType = a.Type,
                                ContentUrl = a.Payload.Url
                            })
                            .ToList()
                });
        }
    }
}