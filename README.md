ASP.Net MVC utilities and helpers for Slack
==============
You have a slack team already set up and want to integrate parts of your ASP.NET MVC application into it so that certain triggers result in Slack posts. These integrations will help you with that.

### Requirements:

1. You must first enable the Webhooks integration for your Slack account to get the token. You can enable it here: https://slack.com/services/new/incoming-webhook
2. This depends on Slack.Webhooks

<!-- NOT YET, DON'T KNOW HOW TO CREATE A NUGET PACKAGE YET
Download:

Package is hosted on [Nuget](https://www.nuget.org/packages/aspnet-mvc-slack/) and can be installed from the package manager:

```
PM> Install-Package aspnet-mvc-slack
```
-->
## WebHookErrorReportFilter
An MVC IExceptionFilter implementation that will post a captured exception to a slack channel.  In order to be effective, exceptions must be allowed to bubble up to the top of your application's ASP.NET MVC process stack so that they are handled by the MVC request pipeline. This is typical for applications that use a common exception handling strategy such as the MVC ```HandleErrorAttribute``` or ASP.NET custom errors behavior.

A couple of import notes:
* This filter only reports the exception. It DOES NOT mark it as handled.
* By default, this filter doesn't care if the exception has already been handled. If you want to ignore previously handled exceptions, set ```IgnoreHandled = true```.

### Example: Global app reporting
Add something like this to your global application configuration (global.asax.xx | App_Start\FilterConfig.xx | etc.)
```csharp
var slackReporter =
	new WebHookErrorReportFilter(
		new WebHookOptions("{my slack team webhook url}")
		{
			ChannelName = '{#channel|@username}", // Optional; Channel/user to post TO
			UserName = "{user name}" // Optional; user to post AS
		}
	)
	{
		IgnoreHandled = true
	};
filters.Add(slackReporter);
```
