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
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class WebHookErrorReportAttribute : FilterAttribute, IWebHookErrorReporter, IExceptionFilter
	{
		private readonly WebHookErrorReportFilter _innerFilter;

		/// <summary>
		/// The slack web hook URL.
		/// </summary>
		public string WebhookUrl
		{
			get { return _innerFilter.WebhookUrl; }
			set { _innerFilter.WebhookUrl = value; }
		}

		/// <summary>
		/// The slack channel name to which exception reports are posted.
		/// Optional; the web hook default channel will be used if not specified.
		/// </summary>
		public string ChannelName
		{
			get { return _innerFilter.ChannelName; }
			set { _innerFilter.ChannelName = value; }
		}

		/// <summary>
		/// The username the error messages will post as.
		/// Optional; the web hook default username will be used if not specified.
		/// </summary>
		public string UserName
		{
			get { return _innerFilter.UserName; }
			set { _innerFilter.UserName = value; }
		}

		public bool IgnoreHandled
		{
			get { return _innerFilter.IgnoreHandled; }
			set { _innerFilter.IgnoreHandled = value; }
		}

		/// <summary>
		/// The types of the exceptions to ignore. Use this to cut down on unecessary channel chatter.
		/// This list will be ignored if <see cref="ExceptionType"/> is specified.
		/// </summary>
		public Type[] IgnoreExceptionTypes
		{
			get { return _innerFilter.IgnoreExceptionTypes; }
			set { _innerFilter.IgnoreExceptionTypes = value; }
		}

		/// <summary>
		/// The Emoji icon used for the posts.
		/// Default is :heavy_exclamation_mark:
		/// </summary>
		public string IconEmoji
		{
			get { return _innerFilter.IconEmoji; }
			set { _innerFilter.IconEmoji = value; }
		}

		/// <summary>
		/// Creates a new instance of the exception filter for the specified <paramref name="webHookUrl"/>.
		/// </summary>
		/// <param name="exceptionType">The exception type to handle.</param>
		public WebHookErrorReportAttribute(string webHookUrl)
		{
			_innerFilter = new WebHookErrorReportFilter(webHookUrl);
		}

		public void OnException(ExceptionContext filterContext)
		{
			_innerFilter.OnException(filterContext);
		}

	}
}
