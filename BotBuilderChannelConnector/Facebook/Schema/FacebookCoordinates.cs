using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Builder.ChannelConnector.Facebook.Schema
{
    public class FacebookCoordinates
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }
        [JsonProperty("long")]
        public double Long { get; set; }
    }
}
