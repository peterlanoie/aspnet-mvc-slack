using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pelasoft.AspNet.Mvc.Slack;
using Webhooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack.Tests
{
	[TestClass]
	public class WebHookExceptionReporterTests
	{

		[TestMethod]
		public void ClientReportsTrue()
		{
			var client = new MockSlackClient(true);
			var reporter = new WebHookExceptionReporter(client);
			Assert.IsTrue(reporter.ReportException(new Exception(), TestHelpers.GetMinimalOptions()));
		}

		[TestMethod]
		public void MessageHasAttachment()
		{
			var client = new MockSlackClient(x => Assert.IsTrue(x.Attachments.Count > 0));
			var reporter = new WebHookExceptionReporter(client);
			reporter.ReportException(new Exception(), TestHelpers.GetMinimalOptions());
		}

		[TestMethod]
		public void MessageDefaultsAreCorrect()
		{
			var client = new MockSlackClient(x =>
			{
				Assert.IsNull(x.Channel);
				Assert.IsNull(x.Username);
				Assert.AreEqual(WebHookExceptionReporter.DEFAULT_TEXT, x.Text);
				Assert.AreEqual(WebHookExceptionReporter.DEFAULT_COLOR, x.Attachments[0].Color);
				Assert.AreEqual(Webhooks.Emoji.HeavyExclamationMark, x.IconEmoji);
				Assert.AreEqual("text", x.Attachments[0].MrkdwnIn.First());
			});
			var reporter = new WebHookExceptionReporter(client);
			Assert.IsTrue(reporter.ReportException(new Exception(), TestHelpers.GetMinimalOptions()));
		}

		[TestMethod]
		public void AttachmentTextContainsExceptionMessage()
		{
			const string message = "This is a test exception.";
			var client = new MockSlackClient(x => Assert.IsTrue(x.Attachments[0].Text.Contains(message)));
			var reporter = new WebHookExceptionReporter(client);
			reporter.ReportException(new Exception(message), TestHelpers.GetMinimalOptions());
		}

		[TestMethod]
		public void AttachmentFallbackContainsExceptionMessage()
		{
			const string message = "This is a test exception.";
			var client = new MockSlackClient(x => Assert.IsTrue(x.Attachments[0].Fallback.Contains(message)));
			var reporter = new WebHookExceptionReporter(client);
			reporter.ReportException(new Exception(message), TestHelpers.GetMinimalOptions());
		}

		[TestMethod]
		public void OptionValuesAreInMessage()
		{
			const string channelName = "#testChannel";
			const string userName = "@testUser";
			const string emoji = "emoji";
			const string text = "message text";
			const string attColor = "attachment color";
			const string attTitle = "attachment title";
			const string attTitleLink = "attachment title link";
			var client = new MockSlackClient(x =>
			{
				Assert.AreEqual(channelName, x.Channel);
				Assert.AreEqual(userName, x.Username);
				Assert.AreEqual(emoji, x.IconEmoji);
				Assert.AreEqual(text, x.Text);
				Assert.AreEqual(attColor, x.Attachments[0].Color);
				Assert.AreEqual(attTitle, x.Attachments[0].Title);
				Assert.AreEqual(attTitleLink, x.Attachments[0].TitleLink);

			});
			var reporter = new WebHookExceptionReporter(client);
			var options = TestHelpers.GetMinimalOptions();
			options.ChannelName = channelName;
			options.UserName = userName;
			options.IconEmoji = emoji;
			options.Text = text;
			options.AttachmentColor = attColor;
			options.AttachmentTitle = attTitle;
			options.AttachmentTitleLink = attTitleLink;
			reporter.ReportException(new Exception(), options);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void NullOptionsThrowsError()
		{
			var client = new MockSlackClient(true);
			var reporter = new WebHookExceptionReporter(client);
			reporter.ReportException(new Exception(), null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void NullWebHookUrlThrowsError()
		{
			var client = new MockSlackClient(true);
			var reporter = new WebHookExceptionReporter(client);
			reporter.ReportException(new Exception(), new WebHookOptions(null));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void EmptyWebHookUrlThrowsError()
		{
			var client = new MockSlackClient(true);
			var reporter = new WebHookExceptionReporter(client);
			reporter.ReportException(new Exception(), new WebHookOptions(""));
		}
		
	}
}
