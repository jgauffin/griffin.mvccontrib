using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Griffin.MvcContrib.Html.Generators;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Facade to be able to use the Helpers as drop in replacements to the ones in MVC3. (Just do a replace all)
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    // All enumerations are safe to be executed multiple times.
    // ReSharper disable PossibleMultipleEnumeration
    public class InputHtmlHelper<TModel> : HtmlHelperFor<TModel>
    {
        private readonly CheckBoxGenerator _checkBoxGenerator;
        private readonly RadioButtonGenerator _radioButtonGenerator;
        private readonly SelectGenerator _selectGenerator;
        private readonly TextAreaGenerator _textAreaGenerator;
        private readonly TextBoxGenerator _textBoxGenerator;
        private HiddenInputGenerator _hiddenInputGenerator;
        private PasswordInputGenerator _passwordInputGenerator;


        /// <summary>
        /// Initializes a new instance of the <see cref="InputHtmlHelper&lt;TModel&gt;"/> class.
        /// </summary>
        /// <param name="helper">The helper.</param>
        public InputHtmlHelper(HtmlHelper<TModel> helper)
            : base(helper)
        {
            var resolver = DependencyResolver.Current;
            _textBoxGenerator = resolver.GetService<TextBoxGenerator>() ?? new TextBoxGenerator(helper.ViewContext);
            _textAreaGenerator = resolver.GetService<TextAreaGenerator>() ?? new TextAreaGenerator(helper.ViewContext);
            _checkBoxGenerator = resolver.GetService<CheckBoxGenerator>() ?? new CheckBoxGenerator(helper.ViewContext);
            _radioButtonGenerator = resolver.GetService<RadioButtonGenerator>() ??
                                    new RadioButtonGenerator(helper.ViewContext);
            _selectGenerator = resolver.GetService<SelectGenerator>() ?? new SelectGenerator(helper.ViewContext);
        }

        public virtual MvcForm BeginForm(string actionName = null, string controllerName = null,
                                         FormMethod method = FormMethod.Post, object routeValues = null,
                                         object htmlAttributes = null)
        {
            var routes = new RouteValueDictionary(routeValues);
            var attributes = new RouteValueDictionary(htmlAttributes);
            var url = UrlHelper.GenerateUrl(null, actionName, controllerName, routes, RouteCollection,
                                            ViewContext.RequestContext, true);

            var tagBuilder = new NestedTagBuilder("form");
            tagBuilder.MergeAttributes(attributes);
            tagBuilder.MergeAttribute("action", url);
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);
            var item = InvokeHtmlTagAdapters(new[] {tagBuilder}).First();

            var httpResponse = ViewContext.HttpContext.Response;
            httpResponse.Write(item.ToString(TagRenderMode.StartTag));
            return new MvcForm(ViewContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        protected virtual GeneratorContext CreateInputContext<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                                    object htmlAttributes = null)
        {
            var metadata = GetMetadata(property);
            var name = ExpressionHelper.GetExpressionText(property);
            var fullName = GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Property name is empty", "name");
            }

            var attributes = htmlAttributes != null
                                 ? new RouteValueDictionary(htmlAttributes)
                                 : new RouteValueDictionary();


            var validationAttributes = GetUnobtrusiveValidationAttributes(name, metadata);
            foreach (var validationAttribute in validationAttributes)
            {
                attributes.Add(validationAttribute.Key, validationAttribute.Value);
            }

            return new GeneratorContext(name, fullName, metadata, attributes);
        }

        /// <summary>
        /// Create an 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public virtual MvcHtmlString EditorFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                          object htmlAttributes = null)
        {
            var context = CreateInputContext(property, htmlAttributes);
            var generatedTags = _textBoxGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        public virtual MvcHtmlString TextBoxFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                           object htmlAttributes = null)
        {
            var context = CreateInputContext(property, htmlAttributes);
            var generatedTags = _textBoxGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        public virtual MvcHtmlString TextAreaFor<TProperty>(Expression<Func<TModel, TProperty>> property, int rows = 5,
                                                            int columns = 40, object htmlAttributes = null)
        {
            var context = CreateInputContext(property, htmlAttributes);
            context.HtmlAttributes.Add("rows", rows);
            context.HtmlAttributes.Add("cols", columns);
            var generatedTags = _textAreaGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }
/*
        public virtual MvcHtmlString DropdownFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                                        IEnumerable<TProperty> items,
                                                                        object selectedItem = null,
                                                                        object htmlAttributes = null)
            where TFormatter : ISelectItemFormatter, new()
        {
            var ctx = CreateInputContext(property, htmlAttributes);
            var context = new SelectContext(ctx, new TFormatter(), items);
            var generatedTags = _selectGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }*/

        public virtual MvcHtmlString DropdownFor<TProperty>(Expression<Func<TModel, TProperty>> property, object htmlAttributes = null)
            where TProperty : struct, IConvertible, IFormattable, IComparable
        {
            var ctx = CreateInputContext(property, htmlAttributes);
            var context = new SelectContext(ctx, null, null);
            var generatedTags = _selectGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();

        }


        /// <summary>
        /// Generate a drop down list.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="items"></param>
        /// <param name="selectedItem"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public virtual MvcHtmlString DropdownFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                            IEnumerable<SelectListItem> items,
                                                            object selectedItem = null, object htmlAttributes = null)
        {
            var ctx = CreateInputContext(property, htmlAttributes);
            var context = new SelectContext(ctx, null, items);
            var generatedTags = _selectGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        /// <summary>
        /// Generate a set of radio buttons using an enum
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="property">Property that the helper is for</param>
        /// <param name="htmlAttributes">Extra HTML attributes</param>
        /// <returns>Generated HTML.</returns>
        public virtual MvcHtmlString CheckBoxFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                            object htmlAttributes = null)
        {
            var context = CreateInputContext(property, htmlAttributes);
            var generatedTags = _checkBoxGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }



        /// <summary>
        /// Generate a radio button
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="property">Property that the helper is for</param>
        /// <param name="value">Value for the ratio button</param>
        /// <param name="htmlAttributes">Extra HTML attributes</param>
        /// <returns>Generated HTML.</returns>
        public virtual MvcHtmlString RadioButtonFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                               object value, object htmlAttributes = null)
        {
            var context = CreateInputContext(property, htmlAttributes);
            context.HtmlAttributes.Add("value", value);
            var generatedTags = _radioButtonGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        /// <summary>
        /// Generate a set of radio buttons using an enum
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="property">Property that the helper is for</param>
        /// <param name="htmlAttributes">Extra HTML attributes</param>
        /// <returns>Generated HTML.</returns>
        public virtual MvcHtmlString RadioButtonsFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                                object htmlAttributes = null)
            where TProperty : struct, IConvertible, IFormattable, IComparable
        {
            var context = CreateInputContext(property, htmlAttributes);
            var generatedTags = _radioButtonGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        /// <summary>
        /// Generate a set of radio buttons using an enum
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="property">Property that the helper is for</param>
        /// <param name="items">A list of different choices</param>
        /// <param name="htmlAttributes">Extra HTML attributes</param>
        /// <returns>Generated HTML.</returns>
        public virtual MvcHtmlString RadioButtonsFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                                IEnumerable items, object htmlAttributes = null)
            where TProperty : struct, IConvertible, IFormattable, IComparable
        {
            var context = CreateInputContext(property, htmlAttributes);
            var generatedTags = _radioButtonGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        /// <summary>
        /// Generate check boxes for a property which is of a enum type.
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="property">Property expression</param>
        /// <param name="htmlAttributes">Optional extra HTML attributes</param>
        /// <returns>Generated HTML string</returns>
        public virtual MvcHtmlString CheckBoxesFor<TProperty>(Expression<Func<TModel, TProperty>> property,
                                                              object htmlAttributes = null)
            where TProperty : struct, IConvertible, IFormattable, IComparable
        {
            var context = CreateInputContext(property, htmlAttributes);
            var generatedTags = _checkBoxGenerator.Generate(context);
            return InvokeFormItemAdapters(context.Metadata, generatedTags).ToMvcString();
            
        }

        /// <summary>
        /// Invoke all adapters that are registered in the inversion of control container.
        /// </summary>
        /// <param name="metadata">Model meta data</param>
        /// <param name="tagBuilders">Tag generated by the HTML helper</param>
        /// <remarks>Uses <see cref="DependencyResolver"/> to find all adapters. Do not forget to register your adapter with a name, or
        /// Unity wont find all adapters that are registered.</remarks>
        protected virtual IEnumerable<NestedTagBuilder> InvokeFormItemAdapters(ModelMetadata metadata,
                                                                               IEnumerable<NestedTagBuilder> tagBuilders)
        {
            var adapters = DependencyResolver.Current.GetServices<IFormItemAdapter>().ToList();
            if (adapters.Count == 0)
                return tagBuilders;

            var tags = new List<NestedTagBuilder>();
            foreach (var context in tagBuilders.Select(tagBuilder => new FormItemAdapterContext(tagBuilder, metadata)))
            {
                foreach (var adapter in adapters)
                {
                    adapter.Process(context);
                }

                tags.Add(context.TagBuilder);
            }
            return tags;
        }

        /// <summary>
        /// Invoke all adapters that are registered in the inversion of control container.
        /// </summary>
        /// <param name="tagBuilders">Tag generated by the HTML helper</param>
        /// <remarks>Uses <see cref="DependencyResolver"/> to find all adapters. Do not forget to register your adapter with a name, or
        /// Unity wont find all adapters that are registered.</remarks>
        protected virtual IEnumerable<NestedTagBuilder> InvokeHtmlTagAdapters(IEnumerable<NestedTagBuilder> tagBuilders)
        {
            var adapters = DependencyResolver.Current.GetServices<IHtmlTagAdapter>().ToList();
            if (adapters.Count == 0)
                return tagBuilders;

            var tags = new List<NestedTagBuilder>();
            foreach (var context in tagBuilders.Select(tagBuilder => new HtmlTagAdapterContext(tagBuilder)))
            {
                foreach (var adapter in adapters)
                {
                    adapter.Process(context);
                }

                tags.Add(context.TagBuilder);
            }
            return tags;
        }
    }

    // ReSharper restore PossibleMultipleEnumeration


    internal class PasswordInputGenerator
    {
    }

    internal class HiddenInputGenerator
    {
    }
}