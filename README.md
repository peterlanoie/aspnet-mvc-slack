ASP.Net MVC utilities and helpers for Slack
==============
[![NuGet Version](http://img.shields.io/nuget/v/aspnet-mvc-slack.svg?style=plastic)](https://www.nuget.org/packages/aspnet-mvc-slack/) [![NuGet Downloads](http://img.shields.io/nuget/dt/aspnet-mvc-slack.svg?style=plastic)](https://www.nuget.org/packages/aspnet-mvc-slack/)

You have a slack team already set up and want to integrate parts of your ASP.NET MVC application into it so that certain triggers result in Slack posts. These integrations will help you with that.

### Requirements:

1. You must first enable the Webhooks integration for your Slack account to get the token. You can enable it here: https://slack.com/services/new/incoming-webhook
2. This depends on Slack.Webhooks

### Compatibilty
The library is built against Framework Version 4.0 since the referenced libraries are built against that. 

#### MVC 3 or higher
The library references `System.Web.Mvc` version 2 so you can use it with MVC 2 and up. However, for version above 3, you may see this error:
```
The given filter instance must implement one or more of the following filter interfaces:
    IAuthorizationFilter, IActionFilter, IResultFilter, IExceptionFilter.
```
This is a bit of a red herring. The runtime is encountering a mismatch for the referenced type `IExceptionFilter` between the one referenced by this library (MVCv2) and the one referenced by your project (> MVCv2).  You need to explicitly direct the runtime to re-bind older MVC references to the one appropriate to your project in order for the referenced types to not conflict.  Ensure that you have a `dependentAssembly` node like this in your web.config (map to the version applicable to your project):

```xml
	<runtime>
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
			<dependentAssembly>
				<assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
				<bindingRedirect oldVersion="0.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
			</dependentAssembly>
		</assemblyBinding>
	</runtime>
```

### Download:

This library is available as a [Nuget package](https://www.nuget.org/packages/aspnet-mvc-slack/). Install it from the package manager console with this command:
```
PM> Install-Package aspnet-mvc-slack
```

## WebHookErrorReportFilter
An MVC IExceptionFilter implementation that will post a captured exception to a slack channel.  In order to be effective, exceptions must be allowed to bubble up to the top of your application's ASP.NET MVC process stack so that they are handled by the MVC request pipeline. This is typical for applications that use a common exception handling strategy such as the MVC ```HandleErrorAttribute``` or ASP.NET custom errors behavior.

A couple of import notes:
* This filter only reports the exception. It DOES NOT mark it as handled.
* By default, this filter doesn't care if the exception has already been handled. If you want to ignore previously handled exceptions, set ```IgnoreHandled = true```.

### Example: Global app reporting
Add something like this to your global application configuration (e.g. global.asax.cs | App_Start\FilterConfig.cs | etc.)
```csharp
var slackReporter =
	new WebHookErrorReportFilter(
		new WebHookOptions("{my slack team webhook url}")
		{
			ChannelName = "{#channel|@username}", // Optional; Channel/user to post TO
			UserName = "{user name}" // Optional; user to post AS
		}
	)
	{
		IgnoreHandled = true
	};
filters.Add(slackReporter);
```
