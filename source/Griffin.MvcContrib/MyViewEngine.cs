namespace Griffin.MvcContrib
{
    /// <summary>
    /// 
    /// </summary>
    public class MyViewEngine : System.Web.Mvc.RazorViewEngine
    {
        public MyViewEngine()
            //  : base()
        {
            this.ViewLocationFormats = new string[]
                                           {
                                               //{0} - Culture Name, {1} - Controller, {2} - Page, {3} Extension (aspx/ascx)
                                               "~/Views/{0}/{1}",
                                               "~/Views/en-US/{1}/{2}{3}",
                                               "~/Views/Shared/{1}/{2}{3}",
                                               "~/Views/Shared/{2}{3}"/**/
                                           };
        }

        public override System.Web.Mvc.ViewEngineResult FindView(System.Web.Mvc.ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return FindView(controllerContext, viewName, masterName, useCache, ".cshtml");
        }

        private System.Web.Mvc.ViewEngineResult FindView(System.Web.Mvc.ControllerContext controllerContext, string viewName, string masterName, bool useCache, string extension)
        {
            if (controllerContext.RequestContext.HttpContext.Request.Url.AbsolutePath.StartsWith("/localization/"))
            {
                return new System.Web.Mvc.ViewEngineResult(CreateView(controllerContext, controllerContext.RequestContext.HttpContext.Request.Url.AbsolutePath + extension, masterName), this);
            }

            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
    }
}