using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotBuilder.ChannelConnector.Facebook;
using Xunit;

namespace BotBuilderChannelConectorTests
{
    public class ActivityTransformationTests
    {
        [Fact]
        public void CanConvertTextMessage()
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = "4711"
                },
                Text = "hello world"
            };

            var fbMessages = activity.ToFacebookMessaging().ToList();

            Assert.Equal(1, fbMessages.Count);
            Assert.Equal("4711", fbMessages.First().Recipient.Id);
            Assert.Equal("hello world", fbMessages.First().Message.Text);
        }

        [Fact]
        public void CanConvertLocationQuickReply()
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = "4711"
                },
                ChannelData = new FacebookMessage
                (
                    "Please enter your location",
                    new List<FacebookQuickReply>
                    {
                        new FacebookQuickReply
                        (
                            FacebookQuickReply.ContentTypes.Location,
                            default(string),
                            default(string)
                        )
                    }
                )
            };

            var fbMessages = activity.ToFacebookMessaging().ToList();
            var fbMessage = fbMessages.First();

            Assert.Equal(1, fbMessages.Count);
            Assert.Equal("4711", fbMessage.Recipient.Id);
            Assert.Equal("Please enter your location", fbMessage.Message.Text);
            Assert.Equal(FacebookQuickReply.ContentTypes.Location, fbMessage.Message.QuickReplies.First().ContentType);
        }

        [Fact]
        public void CanConvertTyping()
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = "4711"
                },
                Type = ActivityTypes.Typing
            };

            var fbMessages = activity.ToFacebookMessaging().ToList();

            Assert.Equal(1, fbMessages.Count);
            Assert.Equal("4711", fbMessages.First().Recipient.Id);
            Assert.Equal("typing_on", fbMessages.First().SenderAction);
        }

        [Fact]
        public void CanConvertQuickReply()
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = "4711"
                },
                ChannelData = new FacebookMessage("quickreplies")
                {
                    QuickReplies = new List<FacebookQuickReply>
                    {
                        new FacebookQuickReply("text", "hello", "hello"),
                        new FacebookQuickReply("text", "hello2", "hello2"),
                    }
                }
            };

            var fbMessages = activity.ToFacebookMessaging().ToList();
            var fbMessage = fbMessages.First();

            Assert.Equal(1, fbMessages.Count);
            Assert.Equal("4711", fbMessage.Recipient.Id);
            Assert.Equal("quickreplies", fbMessage.Message.Text);
            Assert.Equal(FacebookQuickReply.ContentTypes.Text, fbMessage.Message.QuickReplies.First().ContentType);
        }

        [Fact]
        public void CanConvertAttachment()
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = "4711"
                },
                Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        ContentType = "image/png",
                        ContentUrl = "http://aihelpwebsite.com/portals/0/Images/AIHelpWebsiteLogo_Large.png",
                        Name = "AIHelpWebsiteLogo_Large.png"
                    }
                }
            };

            var fbMessages = activity.ToFacebookMessaging().ToList();

            Assert.Equal(1, fbMessages.Count);
            Assert.Equal("4711", fbMessages.First().Recipient.Id);
            Assert.Null(fbMessages.First().Message.Text);
        }

        [Fact]
        public void CanConvertHeroCard()
        {
            var activity = new Activity
            {
                Recipient = new ChannelAccount
                {
                    Id = "4711"
                },
                Attachments = new List<Attachment>
                {
                    new HeroCard
                    {
                        Title = "Deco!",
                        Subtitle = "I am Brasilian",
                        Images = new List<CardImage>
                        {
                            new CardImage(url: "https://upload.wikimedia.org/wikipedia/commons/5/5c/Chelsea_Deco.jpg")
                        },
                        Buttons = new List<CardAction>
                        {
                            new CardAction
                            {
                                Value = "https://de.wikipedia.org/wiki/Deco",
                                Type = ActionTypes.OpenUrl,
                                Title = "Get me to Deco"
                            },
                            new CardAction
                            {
                                Value = "Postback",
                                Type = ActionTypes.PostBack,
                                Title = "Get me to Deco"
                            }

                        }
                    }
                    .ToAttachment()
                }
            };

            var fbMessages = activity.ToFacebookMessaging().ToList();
            var fbMessage = fbMessages.First();
            var template = fbMessage.Message.Attachment.Payload.Elements.First();

            Assert.Equal(1, fbMessages.Count);
            Assert.Equal("4711", fbMessages.First().Recipient.Id);
            Assert.Null(fbMessage.Message.Text);
            Assert.Equal("postback", template.Buttons[1].Type);
        }
    }
}
