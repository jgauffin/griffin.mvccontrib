using System.ComponentModel.DataAnnotations;

namespace Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes
{
    public class TranslateModel
    {
        [Required]
        public string TextKey { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
