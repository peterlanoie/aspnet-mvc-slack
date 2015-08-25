using System;
namespace Pelasoft.AspNet.Mvc.Slack
{
	public class WebHookOptions
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
		/// The Emoji icon used for the posts.
		/// Default is :heavy_exclamation_mark:
		/// </summary>
		public string IconEmoji { get; set; }

		/// <summary>
		/// An optional title for the exception attachment.
		/// </summary>
		public string AttachmentTitle { get; set; }

		/// <summary>
		/// An optional link for the exception attachment title. Must set AttachmentTitle to use this.
		/// </summary>
		public string AttachmentTitleLink { get; set; }

		/// <summary>
		/// The text of the post. A default is provided.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// The color of the attachment bar. Useful to differentiate between 
		/// different sources when posted to the same channel.
		/// </summary>
		public string AttachmentColor { get; set; }

		public WebHookOptions(string webHookUrl)
		{
			WebhookUrl = webHookUrl;
		}
	}
}
