using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Used to generate a textarea HTML element.
    /// </summary>
    public class TextAreaGenerator : FormTagGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextAreaGenerator"/> class.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        public TextAreaGenerator(ViewContext viewContext) : base(viewContext)
        {
        }

        /// <summary>
        /// Generates the tags.
        /// </summary>
        /// <returns>A textarea with a title attribute if Watermark metadata is set.</returns>
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