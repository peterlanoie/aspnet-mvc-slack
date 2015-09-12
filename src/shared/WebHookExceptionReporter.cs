using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using WebHooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack
{
	public class WebHookExceptionReporter
	{
		private readonly ISlackClient _client;

		public WebHookExceptionReporter(string webHookUrl)
			: this(new SlackClientAdapter(new WebHooks.SlackClient(webHookUrl)))
		{
		}

		public WebHookExceptionReporter(ISlackClient client)
		{
			_client = client;
		}

		public bool ReportException(Exception ex, WebHookOptions options)
		{
			if(options == null)
			{
				throw new NullReferenceException(
					"An instance of WebHookOptions must be provided as it contains the details for connecting to the Slack web hook.");
			}
			if(options.WebhookUrl == null)
			{
				throw new ArgumentException(
					"WebHookOptions.WebhookUrl must contain a value. Please provide the URL to your Slack team webhook.");
			}

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

			message.Text = options.Text ?? "An exception has occurred in an application.";

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

			return _client.Post(message);
		}
	}
}
