using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Griffin.MvcContrib.Html;
using Griffin.MvcContrib.Localization;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Base page adding support for the new helpers in all views.
    /// </summary>
    public abstract class GriffinWebViewPage<TModel> : WebViewPage<TModel> 
    {
        private FormHtmlHelper<TModel> _formHelper;
        private TextHtmlHelper<TModel> _textHelper;

        /// <summary>
        /// Gets the text helper.
        /// </summary>
        public TextHtmlHelper<TModel> Text
        {
            get { return _textHelper; }
        }

        

        /// <summary>
        /// Gets the new HTML helpers.
        /// </summary>
        public FormHtmlHelper<TModel> Html2
        {
            get { return _formHelper; }
        }

        /// <summary>
        /// Inits the helpers.
        /// </summary>
        public override void InitHelpers()
        {
            base.InitHelpers();
            _formHelper = new FormHtmlHelper<TModel>(Html);
            _textHelper = new TextHtmlHelper<TModel>(Html);
        }


        /// <summary>
        /// GetText inspired localization
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public MvcHtmlString T(string text)
        {
            return MvcHtmlString.Create(ViewLocalizer.Current.Translate((string)ViewContext.RouteData.Values["Controller"], (string)ViewContext.RouteData.Values["Action"], text));
        }

        public SelectHelper Select
        {
            get{return new SelectHelper();}
        }

    }

    /// <summary>
    /// Required to be able to switch page in the Views\Web.Config, but isn't extended with any new stuff.
    /// </summary>
    public abstract class GriffinWebViewPage : WebViewPage
    {
        
    }

    public class SelectHelper
    {
        public IEnumerable<SelectListItem> From<TTemplate>(IEnumerable items) where TTemplate : ISelectItemFormatter,new()
        {
            var selectItems = new List<SelectListItem>();
            var template = new TTemplate();
            foreach (var item in items)
            {
                selectItems.Add(template.Generate(item));
            }
            return selectItems;
        }
    }
}
