using Griffin.MvcContrib.Localization.Types;
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
    public class LocalizedType
    {
        public int Id { get; set; }
        public int LocaleId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }
        [Required]
        [MaxLength(255)]
        public string TypeName { get; set; }
        [Required]
        [MaxLength(255)]
        public string TextName { get; set; }
        public DateTime UpdatedAt { get; set; }
        [MaxLength(50)]
        public string UpdatedBy { get; set; }
        public string Value { get; set; }

        internal TypePrompt ToTypePrompt()
        {
            // Convert assembly qualified to just full typename
            var fullName = this.TypeName;
            int pos = fullName.IndexOf(",");
            if (pos != -1)
                fullName = fullName.Remove(pos);

            return new TypePrompt
            {
                LocaleId = this.LocaleId,
                TypeFullName = fullName,
                Key = new TypePromptKey(this.Key),
                TextName = this.TextName,
                TranslatedText = this.Value,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy
            };
        }

        internal void Update(TypePromptKey key, string translatedText, CultureInfo culture)
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
    }
}
