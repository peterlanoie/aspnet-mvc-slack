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

			message.Text = string.Format("A web application exception has occurred: {0}", filterContext.Exception.Message);

			client.Post(message);
		}


	}
}
