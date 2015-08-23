using System.Web;
using System.Web.Mvc;

namespace Pelasoft.AspNet.Mvc.Slack.TestWeb
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}