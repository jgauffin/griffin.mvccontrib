using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Base class for all new HtmlHelper facades.
    /// </summary>
    /// <typeparam name="TModel">Strongly typed model type ;)</typeparam>
    public class HtmlHelperFor<TModel>
    {
        private readonly HtmlHelper<TModel> _helper;
        private readonly ViewContext _viewContext;
        private readonly ViewDataDictionary<TModel> _viewData;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlHelperFor&lt;TModel&gt;"/> class.
        /// </summary>
        /// <param name="helper">The helper.</param>
        public HtmlHelperFor(HtmlHelper<TModel> helper)
        {
            _helper = helper;
            _viewData = new ViewDataDictionary<TModel>(helper.ViewDataContainer.ViewData);
            _viewContext = helper.ViewContext;
        }

        protected RouteCollection RouteCollection
        {
            get { return _helper.RouteCollection; }
        }

        /// <summary>
        /// Type of model
        /// </summary>
        protected Type ModelType
        {
            get { return typeof (TModel); }
        }

        /// <summary>
        /// Gets current view data
        /// </summary>
        protected ViewDataDictionary ViewData
        {
            get { return ViewContext.ViewData; }
        }

        public ViewContext ViewContext
        {
            get { return _viewContext; }
        }

        /// <summary>
        /// Get value from the property
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        protected TProperty GetPropertyValue<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            return property.Compile()(_viewData.Model);
        }

        /// <summary>
        /// Get meta data from MVC metadata provider
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual ModelMetadata GetMetadata<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            return ModelMetadata.FromLambdaExpression(property, _viewData);
        }

        /// <summary>
        /// Get validation attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public IDictionary<string, object> GetUnobtrusiveValidationAttributes(string name, ModelMetadata metadata)
        {
            return _helper.GetUnobtrusiveValidationAttributes(name, metadata);
        }

        /// <summary>
        /// Get field name to use in HTML forms
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetFullHtmlFieldName(string name)
        {
            return ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
        }

        /// <summary>
        /// Gets input value either from model state or from the model itself
        /// </summary>
        /// <param name="name"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        /// <remarks>
        /// Will use <see cref="ModelMetadata.EditFormatString"/> if it has been specified.
        /// </remarks>
        protected string GetInputValue(string name, ModelMetadata metadata)
        {
            ModelState modelState;
            if (ViewContext.ViewData.ModelState.TryGetValue(name, out modelState) && modelState.Value != null)
            {
                return Convert.ToString(modelState.Value, CultureInfo.CurrentUICulture);
            }

            return !string.IsNullOrEmpty(metadata.EditFormatString)
                       ? string.Format(metadata.EditFormatString, metadata.Model)
                       : Convert.ToString(metadata.Model, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// Convert lambda expression to a property name (nesting supported)
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="property">Property expression</param>
        /// <returns>Property name as string</returns>
        protected virtual string GetPropertyName<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            MemberExpression me;
            switch (property.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = property.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = property.Body as MemberExpression;
                    break;
            }

            var propertyName = "";
            while (me != null)
            {
                propertyName += me.Member.Name + ".";
                me = me.Expression as MemberExpression;
            }

            return propertyName == "" ? "" : propertyName.Remove(propertyName.Length - 1, 1);
        }
    }
}