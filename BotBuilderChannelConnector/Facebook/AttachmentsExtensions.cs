using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using Vossccp.BotBuilder.ChannelConnector.Facebook.Schema;

namespace Vossccp.BotBuilder.ChannelConnector.Facebook
{
    public static class AttachmentsExtensions
    {
        public static FacebookElement ToFacebookElement(this Attachment attachment)
        {
            if (attachment.ContentType == HeroCard.ContentType)
            {
                var card = attachment.Content as HeroCard;

                return new FacebookElement
                {
                    Title = card.Title,
                    Subtitle = card.Subtitle,
                    ImageUrl = ToImageUrl(card.Images),
                    DefaultAction = ToDefaultAction(card.Tap),
                    Buttons = card.Buttons?.Select(ToFacebookButton).ToList()
                };
            }
            if (attachment.ContentType == KeyboardCard.ContentType)
            {
                // handled directly as channel data
                throw new NotSupportedException(KeyboardCard.ContentType);
            }

            return null;
        }

        public static FacebookAttachment ToLinkAttachment(this Attachment attachment)
        {
            if (attachment.ContentType.StartsWith("image", StringComparison.InvariantCultureIgnoreCase))
            {
                return new FacebookAttachment
                {
                    Type = "image",
                    Payload = new FacebookPayload
                    {
                        Url = attachment.ContentUrl
                    }
                };
            }
            if (attachment.ContentType.StartsWith("audio", StringComparison.InvariantCultureIgnoreCase))
            {
                return new FacebookAttachment
                {
                    Type = "audio",
                    Payload = new FacebookPayload
                    {
                        Url = attachment.ContentUrl
                    }
                };
            }
            if (attachment.ContentType.StartsWith("video", StringComparison.InvariantCultureIgnoreCase))
            {
                return new FacebookAttachment
                {
                    Type = "video",
                    Payload = new FacebookPayload
                    {
                        Url = attachment.ContentUrl
                    }
                };
            }

            return null;
            //if (attachment.ContentType.StartsWith("application", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return new FacebookAttachment
            //    {
            //        Type = "file",
            //        Payload = new FacebookPayload
            //        {
            //            Url = attachment.ContentUrl
            //        }
            //    };
            //}
        }

        static FacebookButton ToDefaultAction(CardAction cardAction)
        {
            if (cardAction == null)
            {
                return null;
            }

            var url = string.Empty;
            if (cardAction.Value is string)
            {
                url = cardAction.Value as string;
            }

            return new FacebookButton
            {
                Type = "web_url",
                Url = url
            };
        }

        static string ToImageUrl(IList<CardImage> images)
        {
            var firstImage = images?.FirstOrDefault();
            if (firstImage == null)
            {
                return null;
            }
            return firstImage.Url;
        }

        static FacebookButton ToFacebookButton(CardAction cardAction)
        {
            if(cardAction.Type == ActionTypes.OpenUrl)
            {
                return new FacebookButton
                {
                    Type = "web_url",
                    Title = cardAction.Title,
                    Url = cardAction.Value?.ToString()
                };
            }

            if(cardAction.Type==ActionTypes.PostBack)
            {
                return new FacebookButton
                {
                    Type = "postback",
                    Title = cardAction.Title,
                    Payload = cardAction.Value?.ToString()
                };
            }
            throw new NotSupportedException($"{cardAction.Type} not supported");
        }
    }
}
