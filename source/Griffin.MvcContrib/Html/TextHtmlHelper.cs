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
            return null;
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



        public TagBuilder TagBuilder
        {
            get
            {
                return _tagBuilder;
            }
        }
    }

    public class TextHtmlHelperAdapterContext : ITextHtmlHelperAdapterContext
    {
        private readonly ModelMetadata _metadata;

        public TextHtmlHelperAdapterContext(ModelMetadata metadata)
        {
            _metadata = metadata;
        }

        public ModelMetadata Metadata
        {
            get
            {
                return _metadata;
            }
        }
    }
}