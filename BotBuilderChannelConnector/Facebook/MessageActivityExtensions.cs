using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Bot.Builder.ChannelConnector.Facebook.Schema;
using Newtonsoft.Json.Linq;

namespace Bot.Builder.ChannelConnector.Facebook
{
    public static class MessageActivityExtensions
    {
        public static IEnumerable<IMessageActivity> ToMessageActivities(this FacebookRequestMessage fbRequestMessage)
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
                            .ToList(),

                    Entities = m.Message.Attachments?
                             .Where(a => a.Type == "location")
                             .Select(a => new Entity("Place")
                             {
                                 Properties = JObject.FromObject(new
                                 {
                                     Type = "Place",
                                     Geo = new GeoCoordinates
                                     {
                                         Latitude = a.Payload.Coordinates.Lat,
                                         Longitude = a.Payload.Coordinates.Long
                                     }
                                 })
                             })
                             .ToList()
                });
        }
    }
}