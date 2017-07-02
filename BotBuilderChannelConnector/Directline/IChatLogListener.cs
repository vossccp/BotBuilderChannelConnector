using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Bot.Builder.ChannelConnector.Directline
{
	public interface IChatLogListener
	{
		void OnActivity(Activity activity);
	}
}
