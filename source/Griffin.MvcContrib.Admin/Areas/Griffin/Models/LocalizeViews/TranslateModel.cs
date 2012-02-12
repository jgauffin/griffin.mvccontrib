using System.ComponentModel.DataAnnotations;

namespace Griffin.MvcContrib.Areas.Griffin.Models.LocalizeViews
{
    public class TranslateModel
    {
        [Required]
        public string TextKey { get; set; }

        [Required]
        public string Text { get; set; }
    }
}