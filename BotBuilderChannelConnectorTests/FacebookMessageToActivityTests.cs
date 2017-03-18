using Bot.Builder.ChannelConnector.Facebook;
using Bot.Builder.ChannelConnector.Facebook.Schema;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bot.Builder.ChannelConnector.Tests
{
    public class FacebookMessageToActivityTests
    {
        [Fact]
        public void CanConvertTextMessage()
        {
            var fbMessage = new FacebookRequestMessage
            {
                Entries = new[]
                {
                    new FacebookEntry
                    {
                        Messaging = new []
                        {
                            new FacebookInboundMessaging
                            {
                                Sender = new FacebookAccount
                                {
                                    Id = "1"
                                },
                                Recipient = new FacebookAccount
                                {
                                    Id = "2"
                                },
                                Message = new FacebookInboundMessage
                                {
                                    Mid = "1",
                                    Text = "Hello"
                                }
                            }
                        }
                    }
                }
            };

            var activities = fbMessage.ToMessageActivities().ToList();
            var activity = activities.First();

            Assert.Equal(1, activities.Count);
            Assert.Equal("Hello", activity.Text);
        }

        [Fact]
        public void CanConvertLocation()
        {
            var fbMessage = new FacebookRequestMessage
            {
                Entries = new[]
                {
                    new FacebookEntry
                    {
                        Messaging = new []
                        {
                            new FacebookInboundMessaging
                            {
                                Sender = new FacebookAccount
                                {
                                    Id = "1"
                                },
                                Recipient = new FacebookAccount
                                {
                                    Id = "2"
                                },
                                Message = new FacebookInboundMessage
                                {
                                    Mid = "1",
                                    Attachments = new []
                                    {
                                        new FacebookAttachment
                                        {
                                            Type = "location",
                                            Payload = new FacebookPayload
                                            {
                                                Coordinates = new FacebookCoordinates
                                                {
                                                    Lat = 1d,
                                                    Long = 1d
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var activities = fbMessage.ToMessageActivities().ToList();
            var activity = activities.First();

            var place = activity.Entities
                .Where(t => t.Type == "Place")
                .Select(t => t.GetAs<Place>())
                .First();

            var geo = place.Geo.ToObject<GeoCoordinates>();

            Assert.Equal(1, activities.Count);
            Assert.Equal(1d, geo.Latitude);
            Assert.Equal(1d, geo.Longitude);
        }
    }
}
