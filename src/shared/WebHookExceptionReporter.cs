using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using WebHooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack
{
	public static class WebHookExceptionReporter
	{
		public static void ReportException(Exception ex, WebHookOptions options)
		{
			var client = new WebHooks.SlackClient(options.WebhookUrl);
			var message = new WebHooks.SlackMessage();
			if(!string.IsNullOrEmpty(options.ChannelName))
			{
				message.Channel = options.ChannelName;
			}
			if(!string.IsNullOrEmpty(options.UserName))
			{
				message.Username = options.UserName;
			}

			message.IconEmoji = options.IconEmoji ?? WebHooks.Emoji.HeavyExclamationMark;

			//			message.Text = string.Format("A web application exception has occurred:\n   type: {0}\n   message: {1}\n   stack trace: {2}", ex.GetType(), ex.Message, ex.StackTrace);
			message.Text = options.Text ?? "An exception has occurred in an MVC application.";

			var attachment = new WebHooks.SlackAttachment();
			// simple message for unformatted and notification views
			attachment.Fallback = string.Format("Web app exception: {0}", ex.Message);
			attachment.Color = options.AttachmentColor ?? "danger";

			if(!string.IsNullOrEmpty(options.AttachmentTitle))
			{
				attachment.Title = options.AttachmentTitle;
			}
			if(!string.IsNullOrEmpty(options.AttachmentTitleLink))
			{
				attachment.TitleLink = options.AttachmentTitleLink;
			}
			attachment.MrkdwnIn = new List<string> { "text" };
			var textFormat = @"*URL*: %%url%%
*Machine Name*: %%hostname%%
*Type*: %%ex:type%%
*Message*: %%ex:message%%
*Target Site*: %%ex:site%%
*Stack Trace*:
%%ex:stackTrace%%";

			attachment.Text = textFormat
				.Replace("%%url%%", HttpContext.Current.Request.Url.ToString())
				.Replace("%%hostname%%", Environment.MachineName)
				.Replace("%%ex:type%%", ex.GetType().ToString())
				.Replace("%%ex:message%%", ex.Message)
				.Replace("%%ex:site%%", ex.TargetSite.ToString())
				.Replace("%%ex:stackTrace%%", ex.StackTrace);

			message.Attachments = new List<WebHooks.SlackAttachment>();
			message.Attachments.Add(attachment);

			client.Post(message);
		}
	}
}
