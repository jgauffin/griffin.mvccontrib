using System.Collections.Generic;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    internal class TextBoxGenerator : FormTagGenerator
    {
        public TextBoxGenerator(ViewContext viewContext) : base(viewContext)
        {
        }

        protected override IEnumerable<NestedTagBuilder> GenerateTags()
        {
            var tag = CreatePrimaryTag("input");
            tag.MergeAttribute("type", "text");
            tag.MergeAttribute("value", GetValue());
            if (!string.IsNullOrEmpty(Context.Metadata.Watermark))
                tag.MergeAttribute("title", Context.Metadata.Watermark);

            return new[] {tag};
        }
    }
}