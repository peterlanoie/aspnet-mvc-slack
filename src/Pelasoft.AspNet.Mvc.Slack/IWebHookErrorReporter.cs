using System;
namespace Pelasoft.AspNet.Mvc.Slack
{
	interface IWebHookErrorReporter
	{
		string ChannelName { get; set; }
		Type[] ExcludeExceptionTypes { get; set; }
		bool IgnoreHandled { get; set; }
		string UserName { get; set; }
		string WebhookUrl { get; set; }
		string IconEmoji { get; set; }
	}
}
