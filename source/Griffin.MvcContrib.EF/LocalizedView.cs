using Griffin.MvcContrib.Localization.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Griffin.MvcContrib.EF
{
    public class LocalizedView
    {
        public int Id { get; set; }
        public int LocaleId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Key { get; set; }
        [Required]
        [MaxLength(255)]
        public string ViewPath { get; set; }
        [Required]
        public string TextName { get; set; }
        [Required]
        public string Value { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        [MaxLength(50)]
        public string UpdatedBy { get; set; }

        internal void Update(ViewPromptKey key, string translatedText, System.Globalization.CultureInfo culture)
        {
            Update(key.ToString(), translatedText, culture);
        }

        internal void Update(string key, string translatedText, CultureInfo culture)
        {
            this.Value = translatedText;
            this.UpdatedAt = DateTime.Now;
            this.UpdatedBy = Thread.CurrentPrincipal.Identity.Name;
            this.LocaleId = culture.LCID;
            this.Key = key;
        }

        internal ViewPrompt ToViewPrompt()
        {
            return new ViewPrompt
            {
                LocaleId = this.LocaleId,
                ViewPath = this.ViewPath,
                Key = new ViewPromptKey(this.Key),
                TextName = this.TextName,
                TranslatedText = this.Value
            };
        }
    }
}
