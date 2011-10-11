using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    public class TextAreaGenerator : FormTagGenerator
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

    public class MyCustomTextAreaGenerator : TextAreaGenerator
    {
        public MyCustomTextAreaGenerator(ViewContext viewContext) : base(viewContext)
        {
        }

        protected override IEnumerable<NestedTagBuilder> GenerateTags()
        {
            var generatedTags = base.GenerateTags().ToArray();
            generatedTags[0].MergeAttribute("title", "You are so dirty!");
            return generatedTags;
        }
    }

}