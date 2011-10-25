using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.Admin.Models
{
	public class ViewPrompt
	{
		private readonly TextPrompt _prompt;

		public ViewPrompt(TextPrompt prompt)
		{
			_prompt = prompt;
		}

		public string TextKey { get { return _prompt.TextKey; } }

		/// <summary>
		/// Gets or sets locale id (refer to MSDN)
		/// </summary>
		public int LocaleId { get { return _prompt.LocaleId; } }

		/// <summary>
		/// Gets or sets controller that the text is for
		/// </summary>
		public string ControllerName { get { return _prompt.ControllerName; } }

		/// <summary>
		/// Gets or sets view name (unique in combination with controller name=
		/// </summary>
		public string ActionName { get { return _prompt.ActionName; } }

		/// <summary>
		/// Gets or sets the text to translate
		/// </summary>
		public string TextName { get { return _prompt.TextName; } }

		/// <summary>
		/// Gets or sets translated text
		/// </summary>
		/// <value>Empty string if not translated</value>
		public string TranslatedText { get { return _prompt.TranslatedText; } }

		public CultureInfo Culture { get { return new CultureInfo(_prompt.LocaleId); } }
	}
}