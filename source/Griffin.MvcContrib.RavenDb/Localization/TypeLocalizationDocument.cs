using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Griffin.MvcContrib.Localization.Types;

namespace Griffin.MvcContrib.RavenDb.Localization
{
    internal class TypeLocalizationDocument
    {
        /// <summary>
        /// Gets or sets language code.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets localized prompts
        /// </summary>
        public List<TypePromptDocument> Prompts { get; set; }

        protected bool IsModified { get; set; }

        public TypeLocalizationDocument Clone(CultureInfo newCulture)
        {
            var ourPrompts = (from p in Prompts
                              select new TypePromptDocument(newCulture, p)
                                         {
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

        public void AddPrompt(TypePromptDocument typePrompt)
        {
            Prompts.Add(typePrompt);
            IsModified = true;
        }

        public TypePromptDocument Get(Type model, string propertyName)
        {
            return (from p in Prompts
                    where p.TypeName == model.Name
                          && p.TextName == propertyName
                    select p).FirstOrDefault();
        }

        public void DeletePrompt(TypePromptKey key)
        {
            if (key == null) throw new ArgumentNullException("key");
            Prompts.RemoveAll(k => k.TextKey == key.ToString());
        }
    }
}