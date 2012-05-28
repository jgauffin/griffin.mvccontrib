using System.Collections.Generic;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Generates text inputs where the title has been set to the description
    /// </summary>
    public class TextBoxGenerator : FormTagGenerator
    {
        /// <summary>
        /// Generates the tags.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<NestedTagBuilder> GenerateTags()
        {
            var tag = CreatePrimaryTag("input");
            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("value", GetValue());
            if (!string.IsNullOrEmpty(Context.Metadata.Watermark))
                tag.MergeAttribute("title", Context.Metadata.Description);

            return new[] {tag};
        }
    }
}