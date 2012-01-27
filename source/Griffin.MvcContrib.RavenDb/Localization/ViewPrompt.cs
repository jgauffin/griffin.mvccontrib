using System;
using System.Threading;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	public class ViewPrompt
	{
		public ViewPrompt()
		{
			UpdatedAt = DateTime.Now;
			UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
		}

		public ViewPrompt(TextPrompt prompt)
		{
			ViewPath = prompt.ViewPath;
			UpdatedAt = DateTime.Now;
			UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
			LocaleId = prompt.LocaleId;
			TextKey = prompt.Key.ToString();
			TextName = prompt.TextName;
			Text = prompt.TranslatedText;
		}

		public ViewPrompt(ViewPrompt prompt)
		{
			ViewPath = prompt.ViewPath;
			TextName = prompt.TextName;
			TextKey = prompt.TextKey;
			LocaleId = prompt.LocaleId;
			UpdatedAt = prompt.UpdatedAt;
			UpdatedBy = prompt.UpdatedBy;
			Text = prompt.Text;
		}

		public DateTime UpdatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public int LocaleId { get; set; }
		public string TextKey { get; set; }
		public string TextName { get; set; }
		public string ViewPath { get; set; }
		public string Text { get; set; }
	}
}