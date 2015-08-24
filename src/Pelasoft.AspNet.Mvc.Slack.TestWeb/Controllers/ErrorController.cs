using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pelasoft.AspNet.Mvc.Slack.TestWeb.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult Throw()
		{
			throw new Exception("Test exception: no controller or method attributed.");
		}

	}
}
