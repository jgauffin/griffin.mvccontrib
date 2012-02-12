using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.RavenDb.Localization
{
    [Serializable]
    internal class ViewLocalizationDocument
    {
        /// <summary>
        /// Gets or set language code (en-us)
        /// </summary>
        public string Id { get; set; }

        public List<ViewPromptDocument> Prompts { get; set; }


        public ViewLocalizationDocument Clone(CultureInfo newCulture)
        {
            var ourPrompts = (from p in Prompts
                              select new ViewPromptDocument(p)
                                         {
                                             LocaleId = newCulture.LCID,
                                             UpdatedAt = DateTime.Now,
                                             UpdatedBy =
                                                 Thread.CurrentPrincipal.Identity.Name,
                                             Text = ""
                                         }).ToList();

            return new ViewLocalizationDocument
                       {
                           Id = newCulture.Name,
                           Prompts = ourPrompts
                       };
        }

        /// <summary>
        /// Delete prompt
        /// </summary>
        /// <param name="key">Prompt key</param>
        /// <returns>true if a prompt was found; otherwise false.</returns>
        public bool Delete(ViewPromptKey key)
        {
            return Prompts.RemoveAll(k => k.TextKey == key.ToString()) > 0;
        }
    }
}