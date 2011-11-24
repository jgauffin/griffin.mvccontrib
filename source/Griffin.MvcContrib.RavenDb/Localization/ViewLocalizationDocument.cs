using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	[Serializable]
	class ViewLocalizationDocument
	{
		public string LanguageCode { get; set; }
		public List<ViewPrompt> Prompts { get; set; }


		public ViewLocalizationDocument Clone(CultureInfo newCulture)
		{
			var ourPrompts = (from p in Prompts
							  select new ViewPrompt(p)
							  {
								  LocaleId = newCulture.LCID,
								  UpdatedAt = DateTime.Now,
								  UpdatedBy =
									  Thread.CurrentPrincipal.Identity.Name,
								  Text = ""
							  }).ToList();

			return new ViewLocalizationDocument
			{
				LanguageCode = newCulture.Name,
				Prompts = ourPrompts
			};

		}

	}
}
