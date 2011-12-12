using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			ActionName = prompt.ActionName;
			ControllerName = prompt.ControllerName;
			UpdatedAt = DateTime.Now;
			UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
			LocaleId = prompt.LocaleId;
			TextKey = prompt.TextKey;
			TextName = prompt.TextName;
			Text = prompt.TranslatedText;
		}
		public ViewPrompt(ViewPrompt prompt)
		{
			ActionName = prompt.ActionName;
			ControllerName = prompt.ControllerName;
			TextName = prompt.TextName;
			TextKey = prompt.TextKey;
			LocaleId = prompt.LocaleId;
			UpdatedAt = prompt.UpdatedAt;
			UpdatedBy = prompt.UpdatedBy;
			Text = prompt.Text;
		}

		public string ControllerName { get; set; }

		public DateTime UpdatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public int LocaleId { get; set; }
		public string TextKey { get; set; }
		public string TextName { get; set; }
		public string ActionName { get; set; }
		public string Text { get; set; }
	}
}
