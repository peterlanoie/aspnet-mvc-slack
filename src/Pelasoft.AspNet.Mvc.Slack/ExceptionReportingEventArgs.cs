using System;

namespace Pelasoft.AspNet.Mvc.Slack
{
	public class ExceptionReportingEventArgs
	{
		/// <summary>
		/// The exception to be reported.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Web hook options to use for the report.
		/// </summary>
		public WebHookOptions Options { get; set; }

		/// <summary>
		/// Whether or not to cancel the current exception report.
		/// Use this for when you need to selectively cancel a report.
		/// </summary>
		public bool CancelReport { get; set; }

		public ExceptionReportingEventArgs(Exception exception)
		{
			Exception = exception;
		}
	}
}