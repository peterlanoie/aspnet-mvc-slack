using System.Web;
using System.Web.Mvc;
using Pelasoft.AspNet.Mvc.Slack;
using System.Configuration;

namespace Pelasoft.AspNet.Mvc.Slack.TestWeb
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());

			var slackReport =
				new WebHookErrorReportFilter(ConfigurationManager.AppSettings["slack:webhookurl"])
				{
					ChannelName = ConfigurationManager.AppSettings["slack:channel"],
					UserName = ConfigurationManager.AppSettings["slack:username"],
					IconEmoji = ConfigurationManager.AppSettings["slack:iconEmoji"],
					IgnoreHandled = true,
				};
			filters.Add(slackReport, 1);
		}
	}
}