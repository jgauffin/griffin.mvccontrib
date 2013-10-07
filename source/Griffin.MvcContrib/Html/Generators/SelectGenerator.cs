using Griffin.MvcContrib.Localization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Used to generate SELECT tags
    /// </summary>
    public class SelectGenerator : FormTagGenerator
    {
        protected override IEnumerable<NestedTagBuilder> GenerateTags()
        {
            var type = getNonNullableModelType();

            if (typeof (Enum).IsAssignableFrom(type))
                return GenerateForEnum(Context);

            var selectContext = Context as SelectContext;
            if (selectContext == null)
                throw new InvalidOperationException("Only SelectContext is supported.");

            var tagBuilder = CreatePrimaryTag("select");

            var value = GetValue();
            tagBuilder.AddChildren(GenerateOptions(selectContext.ListItems, value, selectContext.Formatter));
            return new[] {tagBuilder};
        }


        public virtual IEnumerable<NestedTagBuilder> GenerateForEnum(ITagBuilderContext context)
        {
            Setup(context);

            var selectTag = CreatePrimaryTag("select");
            if (!string.IsNullOrEmpty(Context.Metadata.Description))
                selectTag.MergeAttribute("title", Context.Metadata.Description);

            var names = Enum.GetNames(getNonNullableModelType());
            if(context.Metadata.IsNullableValueType)
                names = new string[] { string.Empty }.Concat(names).ToArray();

            foreach (var enumName in names)
            {
                var tagBuilder = new NestedTagBuilder("option");
                tagBuilder.MergeAttribute("value", enumName, true);
                if (GetValue() == enumName)
                    tagBuilder.MergeAttribute("selected", "selected");

                if (enumName != string.Empty)
                {
                    var title = LocalizedStringProvider.GetEnumString(getNonNullableModelType(), enumName) ?? DefaultUICulture.FormatUnknown(enumName);
                    tagBuilder.SetInnerText(title);
                }

                selectTag.AddChild(tagBuilder);
            }

            return new[] {selectTag};
        }

        private Type getNonNullableModelType()
        {
            return getNonNullableModelType(Context.Metadata);
        }

        /// <remarks>
        /// http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
        /// </remarks>
        private Type getNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type result = Nullable.GetUnderlyingType(modelMetadata.ModelType) ?? modelMetadata.ModelType;
            return result;
        }
    }
}