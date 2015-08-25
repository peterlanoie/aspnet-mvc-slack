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
					AttachmentColor = ConfigurationManager.AppSettings["slack:color"],
					AttachmentTitle = ConfigurationManager.AppSettings["slack:title"],
					AttachmentTitleLink = ConfigurationManager.AppSettings["slack:link"],
					Text = ConfigurationManager.AppSettings["slack:text"],
					IgnoreHandled = true,
					IgnoreExceptionTypes = new [] { typeof(System.ApplicationException) },
				};
			filters.Add(slackReport, 1);
		}
	}
}