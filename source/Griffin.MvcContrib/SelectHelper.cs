using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Griffin.MvcContrib.Html;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectHelper
    {
        public IEnumerable<SelectListItem> From<TTemplate>(IEnumerable items)
            where TTemplate : ISelectItemFormatter, new()
        {
            var selectItems = new List<SelectListItem>();
            var template = new TTemplate();
            foreach (var item in items)
            {
                selectItems.Add(template.Generate(item));
            }
            return selectItems;
        }
    }
}