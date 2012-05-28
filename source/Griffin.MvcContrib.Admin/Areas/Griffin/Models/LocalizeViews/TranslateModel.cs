using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Areas.Griffin.Models.LocalizeViews
{
    public class TranslateModel
    {
        [Required]
        public string TextKey { get; set; }

        [Required, AllowHtml]
        public string Text { get; set; }
    }
}