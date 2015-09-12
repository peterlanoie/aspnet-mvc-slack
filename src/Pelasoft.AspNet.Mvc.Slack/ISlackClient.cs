using System;
using Webhooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack
{
	public interface ISlackClient
	{
		bool Post(Webhooks.SlackMessage slackMessage);
	}
}
