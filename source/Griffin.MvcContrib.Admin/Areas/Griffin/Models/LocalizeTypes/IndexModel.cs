using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Areas.Griffin.Models.LocalizeTypes
{
    public class IndexModel
    {
        public IEnumerable<TypePrompt> Prompts { get; set; }
        public IEnumerable<CultureInfo> Cultures { private get; set; }
        public bool ShowMetadata { get; set; }

        public IEnumerable<SelectListItem> CultureItems
        {
            get
            {
                return Cultures.Select(p => new SelectListItem
                                                {
                                                    Text = p.DisplayName,
                                                    Value = p.Name
                                                });
            }
        }
    }
}