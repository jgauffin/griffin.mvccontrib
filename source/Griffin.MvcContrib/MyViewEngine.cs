using System.Web.Mvc;

namespace Griffin.MvcContrib
{
    /// <summary>
    /// 
    /// </summary>
    public class MyViewEngine : RazorViewEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyViewEngine"/> class.
        /// </summary>
        public MyViewEngine()
            //  : base()
        {
            ViewLocationFormats = new[]
                                      {
                                          //{0} - Culture Name, {1} - Controller, {2} - Page, {3} Extension (aspx/ascx)
                                          "~/Views/{0}/{1}",
                                          "~/Views/en-US/{1}/{2}{3}",
                                          "~/Views/Shared/{1}/{2}{3}",
                                          "~/Views/Shared/{2}{3}" /**/
                                      };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName,
                                                  string masterName, bool useCache)
        {
            return FindView(controllerContext, viewName, masterName, useCache, ".cshtml");
        }

        private ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName,
                                          bool useCache, string extension)
        {
            if (controllerContext.RequestContext.HttpContext.Request.Url.AbsolutePath.StartsWith("/localization/"))
            {
                return
                    new ViewEngineResult(
                        CreateView(controllerContext,
                                   controllerContext.RequestContext.HttpContext.Request.Url.AbsolutePath + extension,
                                   masterName), this);
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}