using System.Web.Routing;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    /// Light weight interface used to provide view translations.
    /// </summary>
    public interface IViewLocalizer
    {
        /// <summary>
        /// Translate a text prompt
        /// </summary>
        /// <param name="routeData">Used to lookup the controller location</param>
        /// <param name="text">Text to translate</param>
        /// <returns></returns>
        string Translate(RouteData routeData, string text);
    }
}