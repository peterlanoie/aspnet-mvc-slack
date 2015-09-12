using System;
using System.Diagnostics;
using Pelasoft.AspNet.Mvc.Slack;
using Slack.Webhooks;

namespace Pelasoft.AspNet.Mvc.Slack.Tests
{
	public class MockSlackClient : ISlackClient
	{
		private readonly bool _returnValue;
		private readonly Action<SlackMessage> _messageCallback;

		public MockSlackClient(bool returnValue)
		{
			_returnValue = returnValue;
		}

		public MockSlackClient()
			: this(true)
		{
		}

		public MockSlackClient(Action<SlackMessage> messageCallback)
			: this()
		{
			_messageCallback = messageCallback;
		}

		public bool Post(SlackMessage slackMessage)
		{
			if(_messageCallback != null)
			{
				_messageCallback(slackMessage);
			}

			Debug.WriteLine("Slack message posted.");
			Debug.WriteLine(string.Format("	  Channel: {0}", slackMessage.Channel));
			Debug.WriteLine(string.Format("	IconEmoji: {0}", slackMessage.IconEmoji));
			Debug.WriteLine(string.Format("	   Mrkdwn: {0}", slackMessage.Mrkdwn));
			Debug.WriteLine(string.Format("	     Text: {0}", slackMessage.Text));
			Debug.WriteLine(string.Format("	 Username: {0}", slackMessage.Username));
			//foreach(var slackAttachment in slackMessage.Attachments)
			//{
			//    Debug.WriteLine(string.Format("	 Attachment: {0}", slackAttachment.Username));

			//}

			//if(_shouldThrowError)
			//{
			//    throw new MockClientException("Mock slack client thrown exception.");
			//}

			return _returnValue;
		}
	}

	public class MockClientException : Exception
	{
		public MockClientException()
		{
		}

		public MockClientException(string message)
			: base(message)
		{
		}
	}
}