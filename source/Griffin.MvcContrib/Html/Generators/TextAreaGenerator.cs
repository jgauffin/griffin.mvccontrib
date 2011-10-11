using System.Collections.Generic;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    internal class TextAreaGenerator : FormTagGenerator
    {
        public TextAreaGenerator(ViewContext viewContext) : base(viewContext)
        {
        }

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