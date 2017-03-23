using Bot.Builder.ChannelConnector.Directline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bot.Builder.ChannelConnector.Tests
{
    public class DirectlineActivityIdTests
    {
        [Fact]
        public void CanParseActivitIds()
        {
            var id = DirectlineActivityId.Parse("LVbXh0dRoyM3bb9hKHS4bx|0000001");
            Assert.Equal("LVbXh0dRoyM3bb9hKHS4bx|0000001", id.ToString());
        }
    }
}
