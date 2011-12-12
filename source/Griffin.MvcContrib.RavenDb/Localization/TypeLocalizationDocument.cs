using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Griffin.MvcContrib.RavenDb.Localization
{
	class TypeLocalizationDocument 
	{
		/// <summary>
		/// Gets or sets language code.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets localized prompts
		/// </summary>
		public List<TypePrompt> Prompts { get; set; }

		public TypeLocalizationDocument Clone(CultureInfo newCulture)
		{
			var ourPrompts = (from p in Prompts
			                  select new TypePrompt(p)
			                         	{
			                         		LocaleId = newCulture.LCID,
			                         		UpdatedAt = DateTime.Now,
			                         		UpdatedBy =
			                         			Thread.CurrentPrincipal.Identity.Name,
			                         		Text = ""
			                         	}).ToList();

			return new TypeLocalizationDocument
					{
						Id = newCulture.Name,
						Prompts = ourPrompts
					};

		}

		public string GetText(Type model, string propertyName)
		{
			return (from p in Prompts
			        where p.TypeName == model.Name
			              && p.TextName == propertyName
			        select p.Text).FirstOrDefault();
		}

		public void AddPrompt(TypePrompt typePrompt)
		{
			Prompts.Add(typePrompt);
			IsModified = true;
		}

		protected bool IsModified { get; set; }

		public TypePrompt Get(Type model, string propertyName)
		{
			return (from p in Prompts
					where p.TypeName == model.Name
						  && p.TextName == propertyName
					select p).FirstOrDefault();
		}
	}
}
