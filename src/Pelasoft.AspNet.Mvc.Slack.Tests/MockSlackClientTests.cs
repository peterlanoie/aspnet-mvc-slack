using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pelasoft.AspNet.Mvc.Slack;
using Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack.Tests
{
	[TestClass]
	public class MockSlackClientTests
	{

		[TestMethod]
		public void MockClientReturnsTrue()
		{
			var client = new MockSlackClient(true);
			var result = client.Post(new SlackMessage());
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void MockClientReturnsFalse()
		{
			var client = new MockSlackClient(false);
			var result = client.Post(new SlackMessage());
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void MockClientCallsCallback()
		{
			var result = false;
			var client = new MockSlackClient(x => result = true);
			client.Post(new SlackMessage());
			Assert.IsTrue(result);
		}

		[TestMethod]
		[ExpectedException(typeof(MockClientException))]
		public void MockClientThrowsException()
		{
			var client = new MockSlackClient(x => { throw new MockClientException(); });
			client.Post(new SlackMessage());
		}

	}
}
