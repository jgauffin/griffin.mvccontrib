using System.Web.Mvc;

namespace Griffin.MvcContrib.Areas.Griffin
{
    public class GriffinAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "griffin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "griffin_default",
                "griffin/{controller}/{action}/{id}",
                new {controller = "griffinhome", action = "Index", id = UrlParameter.Optional},
                new[] {GetType().Namespace + ".Controllers"}
                );
        }
    }
}