using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Griffin.MvcContrib.Areas.Griffin
{
    public static class LinkHelpers
    {
        public static MvcHtmlString ConfirmLink(this HtmlHelper helper, string title, string confirmMessage,
                                                string action, string controller = "", object id = null)
        {
            if (string.IsNullOrEmpty(controller))
                controller = helper.ViewContext.RouteData.Values["controller"].ToString();

            var routeData = id == null ? null : new {id};

            return helper.ActionLink(title, action, controller, routeData,
                                     new {rel = confirmMessage, @class = "confirm-and-post"});
        }
    }
}