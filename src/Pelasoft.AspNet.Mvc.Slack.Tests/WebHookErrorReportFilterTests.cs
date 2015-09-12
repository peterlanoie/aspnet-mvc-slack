using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pelasoft.AspNet.Mvc.Slack;

namespace Pelasoft.AspNet.Mvc.Slack.Tests
{
	[TestClass]
	public class WebHookErrorReportFilterTests
	{

		[TestMethod]
		public void CanCreate()
		{
			var filter = new WebHookErrorReportFilter();
		}

		[TestMethod]
		public void ClientReports()
		{
			var isReported = false;
			var client = new MockSlackClient(x => isReported = true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.OnException(MakeExceptionContext());
			Assert.IsTrue(isReported);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void NullOptionsThrowsError()
		{
			var client = new MockSlackClient(true);
			var filter = new WebHookErrorReportFilter(null, client);
			filter.OnException(MakeExceptionContext());
		}

		[TestMethod]
		public void NoReportIfExceptionHandledWhenIgnoring()
		{
			var isReported = false;
			var client = new MockSlackClient(x => isReported = true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client) { IgnoreHandled = true };
			var context = MakeExceptionContext();
			context.ExceptionHandled = true;
			filter.OnException(context);
			Assert.IsFalse(isReported);
		}

		[TestMethod]
		public void IgnoreExceptionType()
		{
			var isReported = false;
			var client = new MockSlackClient(x => isReported = true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.IgnoreExceptionTypes = new[] { typeof(NullReferenceException), typeof(ArgumentException) };
			var context = MakeExceptionContext();

			isReported = false;
			context.Exception = new NullReferenceException();
			filter.OnException(context);
			Assert.IsFalse(isReported);

			isReported = false;
			context.Exception = new ArgumentException();
			filter.OnException(context);
			Assert.IsFalse(isReported);
		}

		[TestMethod]
		public void DontIgnoreExceptionType()
		{
			var isReported = false;
			var client = new MockSlackClient(x => isReported = true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.IgnoreExceptionTypes = new[] { typeof(NullReferenceException), typeof(ArgumentException) };
			var context = MakeExceptionContext();

			context.Exception = new Exception();
			filter.OnException(context);
			Assert.IsTrue(isReported);
		}

		[TestMethod]
		public void OnExceptionReportingRaisesOnTime()
		{
			var eventFired = false;
			var client = new MockSlackClient(x => Assert.IsTrue(eventFired));
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.OnExceptionReporting += delegate { eventFired = true; };
			Assert.IsFalse(eventFired);
			filter.OnException(MakeExceptionContext());
		}

		[TestMethod]
		public void OnExceptionReportedRaisesOnTime()
		{
			var eventFired = false;
			var client = new MockSlackClient(x => Assert.IsFalse(eventFired));
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.OnExceptionReported += delegate { eventFired = true; };
			Assert.IsFalse(eventFired);
			filter.OnException(MakeExceptionContext());
			Assert.IsTrue(eventFired);
		}

		[TestMethod]
		public void OnExceptionReportedIndicatesSuccess()
		{
			var client = new MockSlackClient(true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.OnExceptionReported += args => Assert.IsTrue(args.ReportSucceeded);
			filter.OnException(MakeExceptionContext());
		}

		[TestMethod]
		public void EventsContainOptions()
		{
			var client = new MockSlackClient(true);
			var options = TestHelpers.GetMinimalOptions();
			var filter = new WebHookErrorReportFilter(options, client);
			filter.OnExceptionReporting += args => Assert.AreSame(options, args.Options);
			filter.OnExceptionReported += args => Assert.AreSame(options, args.Options);
			filter.OnException(MakeExceptionContext());
		}

		[TestMethod]
		public void EventsContainException()
		{
			var client = new MockSlackClient(true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			var exception = new Exception("test exception");
			filter.OnExceptionReporting += args => Assert.AreSame(exception, args.Exception);
			filter.OnExceptionReported += args => Assert.AreSame(exception, args.Exception);
			var context = MakeExceptionContext();
			context.Exception = exception;
			filter.OnException(context);
		}

		[TestMethod]
		public void ReportingCanceledByOnExceptionReporting()
		{
			var isReported = false;
			var client = new MockSlackClient(x => isReported = true);
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.OnExceptionReporting += args => args.CancelReport = true;
			filter.OnException(MakeExceptionContext());
			Assert.IsFalse(isReported);
		}

		[TestMethod]
		public void OnExceptionReportingOverridesOptions()
		{
			var originalOptions = TestHelpers.GetMinimalOptions();
			var newOptions = TestHelpers.GetMinimalOptions();
			Assert.AreNotSame(originalOptions, newOptions);
			var client = new MockSlackClient(true);
			var filter = new WebHookErrorReportFilter(originalOptions, client);
			filter.OnExceptionReporting += args => args.Options = newOptions;
			filter.OnExceptionReported += args =>
			{
				Assert.AreSame(newOptions, args.Options);
				Assert.AreNotSame(originalOptions, args.Options);
			};
			filter.OnException(MakeExceptionContext());
		}
		
		[TestMethod]
		[ExpectedException(typeof(MockClientException))]
		public void ReporterErrorThrown()
		{
			var client = new MockSlackClient(x => { throw new MockClientException(); });
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.ThrowOnFailure = true;
			filter.OnException(MakeExceptionContext());
		}

		[TestMethod]
		public void ReporterErrorIgnored()
		{
			var client = new MockSlackClient(x => { throw new MockClientException(); });
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.ThrowOnFailure = false;
			filter.OnException(MakeExceptionContext());
		}

		[TestMethod]
		[ExpectedException(typeof(MockClientException))]
		public void PostEventContainsReporterError()
		{
			var reporterException = new MockClientException();
			var client = new MockSlackClient(x => { throw reporterException; });
			var filter = new WebHookErrorReportFilter(TestHelpers.GetMinimalOptions(), client);
			filter.OnExceptionReported += delegate(ExceptionReportedEventArgs args)
			{
				Assert.AreSame(reporterException, args.ReportException);
				Assert.IsFalse(args.ReportSucceeded);
			};
			filter.OnException(MakeExceptionContext());
		}


		private ExceptionContext MakeExceptionContext(Exception ex = null)
		{
			return new ExceptionContext { Exception = ex ?? new Exception() }; ;
		}
	}
}
