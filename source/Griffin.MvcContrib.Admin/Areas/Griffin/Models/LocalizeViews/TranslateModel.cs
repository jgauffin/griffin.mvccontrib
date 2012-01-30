using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

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
