using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using Griffin.MvcContrib.Localization.Types;

namespace LocalizationAdmin.Models
{
	public class TypePrompt
	{
		private readonly TextPrompt _prompt;

		public TypePrompt()
		{
		}


		public TypePrompt(TextPrompt prompt)
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
		public string TypeName { get { return _prompt.Subject.FullName; } }

		public string FullTypeName { get { return _prompt.Subject.AssemblyQualifiedName; } }

		public string ModelName {get { return _prompt.Subject.Name; }}
		public string Namespace {get { return _prompt.Subject.Namespace; }}

		/// <summary>
		/// Gets or sets view name (unique in combination with controller name=
		/// </summary>
		public string TextName { get { return _prompt.TextName; } }


		/// <summary>
		/// Gets or sets translated text
		/// </summary>
		/// <value>Empty string if not translated</value>
		public string TranslatedText { get { return _prompt.TranslatedText; } }

		public CultureInfo Culture { get { return new CultureInfo(_prompt.LocaleId); } }
	}

	public class TypeEditModel
	{
		[Required]
		public string TextKey { get; set; }
		[Required]
		public string TranslatedText { get; set; }
	}
}