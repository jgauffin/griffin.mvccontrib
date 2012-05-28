using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Text area generator
    /// </summary>
    public class TextAreaGenerator : FormTagGenerator
    {
        protected override IEnumerable<NestedTagBuilder> GenerateTags()
        {
            var tag = CreatePrimaryTag("textarea");
            tag.SetInnerText(GetValue());
            if (!string.IsNullOrEmpty(Context.Metadata.Watermark))
                tag.MergeAttribute("title", Context.Metadata.Watermark);

            return new[] {tag};
        }
    }
}