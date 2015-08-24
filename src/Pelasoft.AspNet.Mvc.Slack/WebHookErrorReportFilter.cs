﻿using System;
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
	public class WebHookErrorReportFilter : IExceptionFilter
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
		/// The types of the exceptions to ignore. Use this to cut down on unecessary channel chatter.
		/// This list will be ignored if <see cref="ExceptionType"/> is specified.
		/// </summary>
		public Type[] ExcludeExceptionTypes { get; set; }

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

			message.IconEmoji = WebHooks.Emoji.HeavyExclamationMark;

			message.Text = string.Format("A web application exception has occurred: {0}", filterContext.Exception.Message);

			client.Post(message);
		}

	}
}