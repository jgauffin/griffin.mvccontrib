using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Griffin.MvcContrib.Html;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Helper methods for select lists.
    /// </summary>
    public class SelectHelper
    {
        /// <summary>
        /// Generates a select list
        /// </summary>
        /// <typeparam name="TTemplate">Template used to fetch label/value.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>Generated select list items</returns>
        public IEnumerable<SelectListItem> From<TTemplate>(IEnumerable items)
            where TTemplate : ISelectItemFormatter, new()
        {
            var template = new TTemplate();
            return (from object item in items select template.Generate(item)).ToList();
        }
    }
}