using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webhooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack
{
	/// <summary>
	/// Wraps the real slack client.
	/// </summary>
	class SlackClientAdapter : ISlackClient
	{
		private readonly Webhooks.SlackClient _slackClient;

		public SlackClientAdapter(Webhooks.SlackClient slackClient)
		{
			_slackClient = slackClient;
		}

		public bool Post(Webhooks.SlackMessage slackMessage)
		{
			return _slackClient.Post(slackMessage);
		}
	}
}
