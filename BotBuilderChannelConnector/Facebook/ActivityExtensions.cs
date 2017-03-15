using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using Vossccp.BotBuilder.ChannelConnector.Facebook.Schema;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook
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
                        Recipient = new FacebookRecipient
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
                            Recipient = new FacebookRecipient
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
                        foreach (var msg in FromAttachments(activity.Recipient.Id, activity.Attachments))
                        {
                            yield return msg;
                        }
                    }
                }
            }
        }

        static FacebookOutboundMessaging CreateFromChannelData(string recipientId, object channelData)
        {
            var message = channelData as FacebookOutboundMessage;
            if (message != null)
            {
                return new FacebookOutboundMessaging
                {
                    Recipient = new FacebookRecipient
                    {
                        Id = recipientId
                    },
                    Message = message
                };
            }

            var facebookMessage = channelData as Microsoft.Bot.Builder.ConnectorEx.FacebookMessage;
            if (facebookMessage != null)
            {
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
            }

            // TODO: we might want to supprt JSON as string
            throw new NotSupportedException($"{channelData.GetType()} not supported");
        }

        static IEnumerable<FacebookOutboundMessaging> FromAttachments(string recipientId, IList<Attachment> attachments)
        {
            if (!attachments.Any())
            {
                yield break;
            }

            var elements = new List<FacebookElement>();
            var links = new List<FacebookAttachment>();
            foreach (var attachment in attachments)
            {
                var element = attachment.ToFacebookElement();
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
                throw new NotSupportedException($"{attachment.ContentType} is not supported");
            }

            if (elements.Any())
            {
                yield return new FacebookOutboundMessaging
                {
                    Recipient = new FacebookRecipient
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
                        Recipient = new FacebookRecipient
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
