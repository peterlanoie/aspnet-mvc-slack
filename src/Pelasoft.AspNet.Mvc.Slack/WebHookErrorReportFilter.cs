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
	public class WebHookErrorReportFilter : IExceptionFilter
	{

		public WebHookOptions Options { get; set; }

		/// <summary>
		/// Whether or not to ignore already handled exceptions.
		/// If this is set true, the application/controller/method filter order will be significant.
		/// </summary>
		public bool IgnoreHandled { get; set; }

		/// <summary>
		/// The types of the exceptions to ignore. Use this to cut down on unecessary channel chatter.
		/// </summary>
		public Type[] IgnoreExceptionTypes { get; set; }

		/// <summary>
		/// Creates a new instance of the exception filter with the the specified web hook <paramref name="options"/>.
		/// </summary>
		public WebHookErrorReportFilter(WebHookOptions options)
		{
			Options = options;
		}

		/// <summary>
		/// Creates a new instance with no parameters. 
		/// Use this if you need to provide the web hook options via the GetWebHookOptions event.
		/// </summary>
		public WebHookErrorReportFilter()
		{
		}

		public event Action<ExceptionReportingEventArgs> OnExceptionReporting;

		public event Action<Exception, WebHookOptions> OnExceptionReported;

		public void OnException(ExceptionContext filterContext)
		{
			// auto eject if
			// ...ignoring handled exceptions
			if(IgnoreHandled && filterContext.ExceptionHandled) return;

			var exception = filterContext.Exception;

			// ...the exception type is in the ignore list
			if(IgnoreExceptionTypes != null
				&& IgnoreExceptionTypes.Length > 0
				&& IgnoreExceptionTypes.Contains(exception.GetType()))
				return;

			var options = Options;

			// is the event set?
			if (OnExceptionReporting != null)
			{
				var eventArgs = new ExceptionReportingEventArgs(exception);
				OnExceptionReporting(eventArgs);
				// did event handler tell us to cancel the error report?
				if(eventArgs.CancelReport)
				{
					// eject!
					return;
				}

				// if options were provided by event handler...
				if(eventArgs.Options != null)
				{
					//...override the original ones.
					options = eventArgs.Options;
				}
			}

			if(options == null)
			{
				throw new NullReferenceException(
					"The WebHookErrorReportFilter.Options must be set as it contains the details for connecting to the Slack web hook. Use any one of: new WebHookErrorReportFilter(WebHookOptions options); WebHookErrorReportFilter.Options setter; OnExceptionReporting event ExceptionReportingEventArgs.Options property.");
			}

			WebHookExceptionReporter.ReportException(exception, options);

			if(OnExceptionReported != null)
			{
				OnExceptionReported(exception, options);
			}
		}

	}
}
