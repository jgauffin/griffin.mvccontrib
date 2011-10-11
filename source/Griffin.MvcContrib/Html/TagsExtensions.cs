using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Extension methods for Tag collection
    /// </summary>
    public static class TagsExtensions
    {
        /// <summary>
        /// Converts a collection into a html string
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>String</returns>
        public static MvcHtmlString ToMvcString(this IEnumerable<TagBuilder> tags)
        {
            var sb = new StringBuilder();
            foreach (var tag in tags)
            {
                sb.AppendLine(tag.ToString());
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}