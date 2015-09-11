using System;

namespace Pelasoft.AspNet.Mvc.Slack
{
	public class ExceptionReportedEventArgs
	{
		/// <summary>
		/// The exception reported.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Web hook options used to make the report.
		/// </summary>
		public WebHookOptions Options { get; set; }

		/// <summary>
		/// Whether or not the exception report succeeded.
		/// </summary>
		public bool ReportSucceeded { get; set; }

		/// <summary>
		/// If an exception occurs while attempting to report the application exception,
		/// it will be included here.
		/// </summary>
		public Exception ReportException { get; set; }

	}
}