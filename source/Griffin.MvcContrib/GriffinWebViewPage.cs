using System.Configuration;
using System.Web.Mvc;
using Griffin.MvcContrib.Html;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// Base page adding support for the new helpers in all views.
    /// </summary>
    public abstract class GriffinWebViewPage<TModel> : WebViewPage<TModel>
    {
        private ViewLocalizer _viewLocalizer;

        protected virtual ViewLocalizer ViewLocalizer
        {
            get
            {
                if (_viewLocalizer == null)
                {
                    _viewLocalizer = DependencyResolver.Current.GetService<ViewLocalizer>();
                    if (_viewLocalizer == null)
                    {
                        var repos = DependencyResolver.Current.GetService<IViewLocalizationRepository>();
                        if (repos == null)
                            throw new ConfigurationErrorsException(
                                "You must register a ViewLocalizer or an IViewLocalizationRepository in your container.");

                        _viewLocalizer = new ViewLocalizer(repos);
                    }
                }

                return _viewLocalizer;
            }
        }

        /// <summary>
        /// Gets the text helper.
        /// </summary>
        public TextHtmlHelper<TModel> Text { get; private set; }


        /// <summary>
        /// Gets the new HTML helpers.
        /// </summary>
        public FormHtmlHelper<TModel> Html2 { get; private set; }

        /// <summary>
        /// Gets select helper
        /// </summary>
        public SelectHelper Select
        {
            get { return new SelectHelper(); }
        }

        /// <summary>
        /// Inits the helpers.
        /// </summary>
        public override void InitHelpers()
        {
            base.InitHelpers();
            Html2 = new FormHtmlHelper<TModel>(Html);
            Text = new TextHtmlHelper<TModel>(Html);
        }


        /// <summary>
        /// GetText inspired localization
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public MvcHtmlString T(string text)
        {
            return MvcHtmlString.Create(ViewLocalizer.Translate(ViewContext.RouteData, text));
        }
    }

    /// <summary>
    /// Required to be able to switch page in the Views\Web.Config, but isn't extended with any new stuff.
    /// </summary>
    public abstract class GriffinWebViewPage : WebViewPage
    {
    }
}