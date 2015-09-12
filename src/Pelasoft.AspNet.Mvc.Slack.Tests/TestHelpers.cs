using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pelasoft.AspNet.Mvc.Slack;

namespace Pelasoft.AspNet.Mvc.Slack.Tests
{
	class TestHelpers
	{
		internal static WebHookOptions GetMinimalOptions()
		{
			return new WebHookOptions("http://localhost/dummyhookurl");
		}
	}
}
