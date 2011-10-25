using System.Globalization;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Localization.Views
{
	/// <summary>
	/// Used to localize views
	/// </summary>
	/// <remarks>
	/// Create a class and implement <see cref="IViewStringsRepository"/> and register it in your container to use an own repository for the view localization. 
	/// </remarks>
	public class ViewLocalizer
	{
		private static ViewLocalizer _current;
		private IViewStringsRepository _repositoryDontUseDirectly;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewLocalizer"/> class.
		/// </summary>
		public ViewLocalizer()
		{
			DefaultCulture = new CultureInfo(1033);
		}

		/// <summary>
		/// Gets current localizer
		/// </summary>
		public static ViewLocalizer Current
		{
			get { return _current ?? (_current = new ViewLocalizer()); }
			set { _current = value; }
		}

		/// <summary>
		/// Gets or sets default culture
		/// </summary>
		/// <remarks>Set to "en-US" if it has not been specified.</remarks>
		public CultureInfo DefaultCulture { get; set; }

		/// <summary>
		/// Gets repository used to fetch strings
		/// </summary>
		/// <remarks>
		/// Uses <see cref="DependencyResolver"/> to find <see cref="IViewStringsRepository"/>.
		/// </remarks>
		protected IViewStringsRepository Repository
		{
			get
			{
				if (_repositoryDontUseDirectly == null)
				{
					_repositoryDontUseDirectly = DependencyResolver.Current.GetService<IViewStringsRepository>() ??
					                             new ViewStringFileRepository();
				}

				return _repositoryDontUseDirectly;
			}
		}

		/// <summary>
		/// Translate a text prompt
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="actionName"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public virtual string Translate(string controllerName, string actionName, string text)
		{
			if (!Repository.Exists(CultureInfo.CurrentUICulture))
			{
				Repository.CreateForLanguage(CultureInfo.CurrentUICulture, DefaultCulture);
			}

			string textToSay = "";
			var id = TextPrompt.CreateKey(controllerName, actionName, text);
			var prompt = Repository.GetPrompt(CultureInfo.CurrentCulture, id);
			if (prompt == null)
			{
				var templatePrompt = new TextPrompt
				                     	{
				                     		TextKey = id,
				                     		ControllerName = controllerName,
				                     		ActionName = actionName,
				                     		TextName = text
				                     	};
				Repository.CreatePrompt(CultureInfo.CurrentUICulture, templatePrompt, "");
			}
			else
				textToSay = prompt.TranslatedText;

			if (string.IsNullOrEmpty(textToSay))
				textToSay = CultureInfo.CurrentUICulture.Equals(DefaultCulture) ? text : string.Format("{1}:[{0}]", text, CultureInfo.CurrentUICulture);

			return textToSay;
		}
	}
}