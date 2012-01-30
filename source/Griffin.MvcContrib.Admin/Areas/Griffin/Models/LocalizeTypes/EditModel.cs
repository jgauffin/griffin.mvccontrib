using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes
{
    public class EditModel
    {
        public string DefaultText { get; set; }
        public string TextKey { get; set; }
        public int LocaleId { get; set; }
        public string Path { get; set; }
        public string Text { get; set; }
    }
}
