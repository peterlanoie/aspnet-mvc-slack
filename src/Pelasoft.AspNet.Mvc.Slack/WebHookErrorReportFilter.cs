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
		/// <summary>
		/// The options defined for the slack web hook.
		/// </summary>
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
		/// Whether or not to throw an exception if the report fails. Default is true.
		/// Set this to false and use the OnExceptionReported event to more gracefully handle slack reporting failures.
		/// </summary>
		public bool ThrowOnFailure { get; set; }

		/// <summary>
		/// Creates a new instance of the exception filter with the the specified web hook <paramref name="options"/>.
		/// </summary>
		public WebHookErrorReportFilter(WebHookOptions options)
			: this()
		{
			Options = options;
		}

		/// <summary>
		/// Creates a new instance with no parameters. 
		/// Use this if you need to provide the web hook options via the GetWebHookOptions event.
		/// </summary>
		public WebHookErrorReportFilter()
		{
			ThrowOnFailure = true;
		}

		/// <summary>
		/// Event raised prior to reporting event. This gives the caller the opportunity to set the options
		/// as well as to cancel the report that's about to be made.
		/// </summary>
		public event Action<ExceptionReportingEventArgs> OnExceptionReporting;

		/// <summary>
		/// Event raised after the exception is reported. This allows the caller to handle any follow up to the report
		/// is useful for any follow up of a report
		/// as well as to assist the caller in seeing the actual options used for the report.
		/// </summary>
		public event Action<ExceptionReportedEventArgs> OnExceptionReported;

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
			if(OnExceptionReporting != null)
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

			var reportedArgs = new ExceptionReportedEventArgs()
			{
				Options = options,
				Exception = exception
			};
			try
			{
				WebHookExceptionReporter.ReportException(exception, options);
				reportedArgs.ReportSucceeded = true;
			}
			catch(Exception ex)
			{
				reportedArgs.ReportException = ex;
			}
			if(OnExceptionReported != null)
			{
				OnExceptionReported(reportedArgs);
			}
			if(!reportedArgs.ReportSucceeded && ThrowOnFailure)
			{
				throw reportedArgs.ReportException;
			}
		}

	}
}
