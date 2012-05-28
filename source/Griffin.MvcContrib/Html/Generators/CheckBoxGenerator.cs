using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Generates checkboxes
    /// </summary>
    public class CheckBoxGenerator : FormTagGenerator
    {
        protected override IEnumerable<NestedTagBuilder> GenerateTags()
        {
            if (typeof (Enum).IsAssignableFrom(Context.Metadata.ModelType))
                return GenerateForEnum(Context);

            return GenerateForSingleBox();
        }

        private IEnumerable<NestedTagBuilder> GenerateForSingleBox()
        {
            var tags = new NestedTagBuilder[2];

            tags[0] = CreatePrimaryTag("input");
            tags[0].MergeAttribute("type", "checkbox");
            tags[0].MergeAttribute("value", GetValue());
            if (Context.Metadata.Model != null)
            {
                bool isChecked;
                if (Boolean.TryParse(Context.Metadata.Model.ToString(), out isChecked) && isChecked)
                {
                    tags[0].MergeAttribute("checked", "checked");
                }
            }

            // add hidden input
            tags[1] = CreatePrimaryTag("input");
            tags[1].MergeAttribute("type", "hidden");
            tags[1].MergeAttribute("value", "false");

            return tags;
        }


        public virtual IEnumerable<NestedTagBuilder> GenerateForEnum(ITagBuilderContext context)
        {
            Setup(context);

            var tags = new List<NestedTagBuilder>();
            foreach (var enumName in Enum.GetNames(Context.Metadata.ModelType))
            {
                var tagBuilder = CreatePrimaryTag("input");
                tagBuilder.MergeAttribute("type", "checkbox", true);
                if (!string.IsNullOrEmpty(Context.Metadata.Description))
                    tagBuilder.MergeAttribute("title", Context.Metadata.Description);


                if (GetValue() == enumName)
                    tagBuilder.MergeAttribute("checked", "checked");


                var label = new NestedTagBuilder("label");
                var title = new NestedTagBuilder("span");
                title.SetInnerText(LocalizedStringProvider.GetEnumString(Context.Metadata.ModelType, enumName));
                label.AddChild(tagBuilder);
                label.AddChild(title);

                tags.Add(label);
            }

            return tags;
        }
    }
}