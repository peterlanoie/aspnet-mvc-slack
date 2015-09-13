using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using WebHooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack
{
	public class WebHookExceptionReporter
	{
		public const string DEFAULT_TEXT = "An exception has occurred in an application.";
		public const string DEFAULT_COLOR = "danger";
		public const string DEFAULT_FORMAT = @"*URL*: %%url%%
*Machine Name*: %%hostname%%
*Type*: %%ex:type%%
*Message*: %%ex:message%%
*Target Site*: %%ex:site%%
*Stack Trace*:
%%ex:stackTrace%%";

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
			if(string.IsNullOrEmpty(options.WebhookUrl))
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

			message.Text = options.Text ?? DEFAULT_TEXT;

			var attachment = new WebHooks.SlackAttachment();
			// simple message for unformatted and notification views
			attachment.Fallback = string.Format("Web app exception: {0}", ex.Message);
			attachment.Color = options.AttachmentColor ?? DEFAULT_COLOR;

			if(!string.IsNullOrEmpty(options.AttachmentTitle))
			{
				attachment.Title = options.AttachmentTitle;
			}
			if(!string.IsNullOrEmpty(options.AttachmentTitleLink))
			{
				attachment.TitleLink = options.AttachmentTitleLink;
			}
			attachment.MrkdwnIn = new List<string> { "text" };
			var textFormat = !string.IsNullOrWhiteSpace(options.ExceptionTextFormat) ? options.ExceptionTextFormat : DEFAULT_FORMAT;

			var requestUrl = HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : "[no HttpContext available]";

			attachment.Text = textFormat
				.Replace("%%url%%", requestUrl)
				.Replace("%%hostname%%", Environment.MachineName)
				.Replace("%%ex:type%%", ex.GetType().ToString())
				.Replace("%%ex:message%%", ex.Message)
				.Replace("%%ex:site%%", ex.TargetSite != null ? ex.TargetSite.ToString() : "")
				.Replace("%%ex:stackTrace%%", ex.StackTrace);

			message.Attachments = new List<WebHooks.SlackAttachment>();
			message.Attachments.Add(attachment);

			return _client.Post(message);
		}
	}
}
