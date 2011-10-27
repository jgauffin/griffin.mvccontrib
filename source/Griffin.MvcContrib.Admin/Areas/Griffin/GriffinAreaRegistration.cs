using System.Web.Mvc;

namespace Griffin.MvcContrib.Areas.Griffin
{
	public class GriffinAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Griffin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Griffin_default",
				"Griffin/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
