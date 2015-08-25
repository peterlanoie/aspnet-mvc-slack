using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebHooks = Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack
{
	/// <summary>
	/// Defines an action filter that logs thrown exceptions to a Slack channel.
	/// </summary>
	public class WebHookErrorReportFilter : IExceptionFilter, IWebHookErrorReporter
	{

		/// <summary>
		/// The slack web hook URL.
		/// </summary>
		public string WebhookUrl { get; set; }

		/// <summary>
		/// The slack channel name to which exception reports are posted.
		/// Optional; the web hook default channel will be used if not specified.
		/// </summary>
		public string ChannelName { get; set; }

		/// <summary>
		/// The username the error messages will post ask.
		/// Optional; the web hook default username will be used if not specified.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Whether or not to ignore already handled exceptions.
		/// If this is set true, the application/controller/method filter order will be significant.
		/// </summary>
		public bool IgnoreHandled { get; set; }

		/// <summary>
		/// The types of the exceptions to ignore. Use this to cut down on unecessary channel chatter.
		/// This list will be ignored if <see cref="ExceptionType"/> is specified.
		/// </summary>
		public Type[] IgnoreExceptionTypes { get; set; }

		/// <summary>
		/// The Emoji icon used for the posts.
		/// Default is :heavy_exclamation_mark:
		/// </summary>
		public string IconEmoji { get; set; }

		/// <summary>
		/// An optional title for the exception attachment.
		/// </summary>
		public string AttachmentTitle { get; set; }

		/// <summary>
		/// An optional link for the exception attachment title. Must set AttachmentTitle to use this.
		/// </summary>
		public string AttachmentTitleLink { get; set; }

		/// <summary>
		/// The text of the post. A default is provided.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// The color of the attachment bar. Useful to differentiate between 
		/// different sources when posted to the same channel.
		/// </summary>
		public string AttachmentColor { get; set; }

		/// <summary>
		/// Creates a new instance of the exception filter for the specified <paramref name="webHookUrl"/>.
		/// </summary>
		/// <param name="exceptionType">The exception type to handle.</param>
		public WebHookErrorReportFilter(string webHookUrl)
		{
			WebhookUrl = webHookUrl;
		}

		public void OnException(ExceptionContext filterContext)
		{
			// auto eject if
			// ...ignoring handled exceptions
			if(IgnoreHandled && filterContext.ExceptionHandled) return;

			// ...the exception type is in the ignore list
			if(IgnoreExceptionTypes != null
				&& IgnoreExceptionTypes.Length > 0
				&& IgnoreExceptionTypes.Contains(filterContext.Exception.GetType()))
				return;

			var ex = filterContext.Exception;
			var client = new WebHooks.SlackClient(WebhookUrl);
			var message = new WebHooks.SlackMessage();
			if(!string.IsNullOrEmpty(ChannelName))
			{
				message.Channel = ChannelName;
			}
			if(!string.IsNullOrEmpty(UserName))
			{
				message.Username = UserName;
			}

			message.IconEmoji = IconEmoji ?? WebHooks.Emoji.HeavyExclamationMark;

			//			message.Text = string.Format("A web application exception has occurred:\n   type: {0}\n   message: {1}\n   stack trace: {2}", ex.GetType(), ex.Message, ex.StackTrace);
			message.Text = Text ?? "An exception has occurred in an MVC application.";

			var attachment = new WebHooks.SlackAttachment();
			// simple message for unformatted and notification views
			attachment.Fallback = string.Format("Web app exception: {0}", ex.Message);
			attachment.Color = AttachmentColor ?? "danger";

			if(!string.IsNullOrEmpty(AttachmentTitle))
			{
				attachment.Title = AttachmentTitle;
			}
			if(!string.IsNullOrEmpty(AttachmentTitleLink))
			{
				attachment.TitleLink = AttachmentTitleLink;
			}
			attachment.MrkdwnIn = new List<string> { "text" };
			var textFormat = @"*URL*: %%url%%
*Type*: %%ex:type%%
*Message*: %%ex:message%%
*Target Site*: %%ex:site%%
*Stack Trace*:
%%ex:stackTrace%%";

			attachment.Text = textFormat
				.Replace("%%url%%", filterContext.HttpContext.Request.Url.ToString())
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
