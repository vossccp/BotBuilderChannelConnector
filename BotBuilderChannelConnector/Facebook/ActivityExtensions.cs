using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Builder.ChannelConnector.Facebook.Schema;
using System.Net.Mime;

namespace Bot.Builder.ChannelConnector.Facebook
{
    public static class ActivityExtensions
    {
        public static IEnumerable<FacebookOutboundMessaging> ToFacebookMessaging(this Activity activity)
        {
            if (activity.ChannelData != null)
            {
                yield return CreateFromChannelData(activity.Recipient.Id, activity.ChannelData);
            }
            else
            {
                if (ActivityTypes.Typing.Equals(activity.Type))
                {
                    yield return new FacebookOutboundMessaging
                    {
                        Recipient = new FacebookAccount
                        {
                            Id = activity.Recipient.Id
                        },
                        SenderAction = "typing_on"
                    };
                }
                else
                {
                    if (!string.IsNullOrEmpty(activity.Text))
                    {
                        yield return new FacebookOutboundMessaging
                        {
                            Recipient = new FacebookAccount
                            {
                                Id = activity.Recipient.Id
                            },
                            Message = new FacebookOutboundMessage
                            {
                                Text = activity.Text
                            }
                        };
                    }

                    if (activity.Attachments != null)
                    {
                        foreach (var msg in FromAttachments(activity.Recipient.Id, activity.Text, activity.Attachments))
                        {
                            yield return msg;
                        }
                    }
                }
            }
        }

        static FacebookOutboundMessaging CreateFromChannelData(string recipientId, object channelData)
        {
            switch (channelData)
            {
                case FacebookOutboundMessage message:
                    return new FacebookOutboundMessaging
                    {
                        Recipient = new FacebookAccount
                        {
                            Id = recipientId
                        },
                        Message = message
                    };
                case Microsoft.Bot.Builder.ConnectorEx.FacebookMessage facebookMessage:
                    return CreateFromChannelData(recipientId, new FacebookOutboundMessage
                    {
                        Text = facebookMessage.Text,
                        QuickReplies = facebookMessage.QuickReplies
                        .Select(q => new FacebookQuickReply
                        {
                            ContentType = q.ContentType,
                            Title = q.Title,
                            Payload = q.Payload
                        })
                        .ToList()
                    });
                default:
                    // TODO: we might want to supprt JSON as string
                    throw new NotSupportedException($"{channelData.GetType()} not supported");
            }
        }

        static IEnumerable<FacebookOutboundMessaging> FromAttachments(string recipientId, string text, IList<Attachment> attachments)
        {
            if (!attachments.Any())
            {
                yield break;
            }

            var elements = new List<FacebookElement>();
            var links = new List<FacebookAttachment>();
            foreach (var attachment in attachments)
            {
                var element = attachment.ToFacebookElement(text);
                if (element != null)
                {
                    elements.Add(element);
                    continue;
                }

                var link = attachment.ToLinkAttachment();
                if (link != null)
                {
                    links.Add(link);
                    continue;
                }

                if (attachment.ContentType == MediaTypeNames.Text.Plain)
                {
                    // ignore extra text
                    continue;
                }
                throw new NotSupportedException($"{attachment.ContentType} is not supported");
            }

            if (elements.Any())
            {
                yield return new FacebookOutboundMessaging
                {
                    Recipient = new FacebookAccount
                    {
                        Id = recipientId
                    },
                    Message = new FacebookOutboundMessage
                    {
                        Attachment = new FacebookAttachment
                        {
                            Type = "template",
                            Payload = new FacebookPayload
                            {
                                TemplateType = "generic",
                                Elements = elements.ToList()
                            }
                        }
                    }
                };
            }

            if (links.Any())
            {
                foreach (var link in links)
                {
                    yield return new FacebookOutboundMessaging
                    {
                        Recipient = new FacebookAccount
                        {
                            Id = recipientId
                        },
                        Message = new FacebookOutboundMessage
                        {
                            Attachment = link
                        }
                    };
                }
            }
        }
    }
}
