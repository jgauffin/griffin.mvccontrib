using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	/// <summary>
	/// Document used to store localization entry in RavenDb.
	/// </summary>
	public class TypePrompt
	{
		public TypePrompt(TextPrompt prompt)
		{
			AssemblyName = prompt.Subject.Assembly.GetName().Name;
			FullTypeName = prompt.Subject.FullName;
			TextName = prompt.TextName;
			TextKey = prompt.TextKey;
			LocaleId = prompt.LocaleId;
			UpdatedAt = prompt.UpdatedAt;
			UpdatedBy = prompt.UpdatedBy;
			Text = prompt.TranslatedText;
		}

		public string AssemblyName { get; set; }

		public string FullTypeName { get; set; }

		public DateTime UpdatedAt { get; set; }
		public string UpdatedBy { get; set; }
		public int LocaleId { get; set; }
		public string TextKey { get; set; }
		public string TextName { get; set; }
		public string TypeName { get; set; }

		public string Text { get; set; }
	}
}
