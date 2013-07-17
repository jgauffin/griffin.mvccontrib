using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Types;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html
{
    public class TextHtmlHelper<TModel> : HtmlHelperFor<TModel>
    {
        public TextHtmlHelper(HtmlHelper<TModel> helper)
            : base(helper)
        {
        }

        public virtual MvcHtmlString LabelFor<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            var metadata = GetMetadata(property);

            var tb = new TagBuilder("label");
            tb.Attributes.Add("id", "LabelFor" + metadata.PropertyName);
            tb.Attributes.Add("for", metadata.PropertyName);
            if (!string.IsNullOrEmpty(metadata.Description))
                tb.Attributes.Add("title", metadata.Description);
            tb.SetInnerText(GetPropertyValue(property).ToString());


            var adapters = DependencyResolver.Current.GetServices<IHtmlLabelAdapter>();
            if (adapters.Any())
            {
                var context = new SingleTagAdapterContext(tb, metadata);
                foreach (var adapter in adapters)
                    adapter.ProcessLabel(context);
            }

            return MvcHtmlString.Create(tb.ToString(TagRenderMode.SelfClosing));
        }

        public MvcHtmlString DisplayFor<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            var languageProvider = DependencyResolver.Current.GetService<ILocalizedStringProvider>();

            var metadata = ModelMetadata.FromStringExpression("", this.ViewData);
            if (metadata.Model == null)
                return new MvcHtmlString(metadata.NullDisplayText);

            var value = metadata.Model.ToString();
            var result =
            metadata.Model is Enum ?
                languageProvider.GetEnumString(getNonNullableModelType(metadata), value) ?? DefaultUICulture.FormatUnknown(value) :
                languageProvider.GetModelString(metadata.ModelType, value) ?? DefaultUICulture.FormatUnknown(value);

            return new MvcHtmlString(result);
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

    public interface IHtmlLabelAdapter
    {
        MvcHtmlString ProcessLabel(ISingleTagAdapterContext context);
    }

    public interface IHtmlTextAreaAdapter
    {
        MvcHtmlString ProcessTextArea(ISingleTagAdapterContext context);
    }

    public interface ITextHtmlHelperAdapterContext
    {
        ModelMetadata Metadata { get; }
    }

    public interface ISingleTagAdapterContext : ITextHtmlHelperAdapterContext
    {
        TagBuilder TagBuilder { get; }
    }

    public class SingleTagAdapterContext : TextHtmlHelperAdapterContext, ISingleTagAdapterContext
    {
        private readonly TagBuilder _tagBuilder;

        public SingleTagAdapterContext(TagBuilder tagBuilder, ModelMetadata metadata)
            : base(metadata)
        {
            _tagBuilder = tagBuilder;
        }

        #region ISingleTagAdapterContext Members

        public TagBuilder TagBuilder
        {
            get { return _tagBuilder; }
        }

        #endregion
    }

    public class TextHtmlHelperAdapterContext : ITextHtmlHelperAdapterContext
    {
        private readonly ModelMetadata _metadata;

        public TextHtmlHelperAdapterContext(ModelMetadata metadata)
        {
            _metadata = metadata;
        }

        #region ITextHtmlHelperAdapterContext Members

        public ModelMetadata Metadata
        {
            get { return _metadata; }
        }

        #endregion
    }
}