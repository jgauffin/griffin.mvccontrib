using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Griffin.MvcContrib.Localization.Views
{
	/// <summary>
	/// Used to localize views
	/// </summary>
	/// <remarks>
	/// Create a class and implement <see cref="IViewLocalizationRepository"/> and register it in your container to use an own repository for the view localization. 
	/// </remarks>
	public class ViewLocalizer : IViewLocalizer
	{
        private static IViewLocalizer _current;
		private IViewLocalizationRepository _repositoryDontUseDirectly;

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
		public static IViewLocalizer Current
		{
			get { return _current ?? (_current = new ViewLocalizer()); }
			set { _current = value; }
		}

		/// <summary>
		/// Gets or sets default culture
		/// </summary>
		/// <remarks>Set to "en-US" if it has not been specified.</remarks>
		public static CultureInfo DefaultCulture { get; set; }

		/// <summary>
		/// Gets repository used to fetch strings
		/// </summary>
		/// <remarks>
		/// Uses <see cref="DependencyResolver"/> to find <see cref="IViewLocalizationRepository"/>.
		/// </remarks>
		protected IViewLocalizationRepository Repository
		{
			get
			{
				return _repositoryDontUseDirectly ??
				       (_repositoryDontUseDirectly = DependencyResolver.Current.GetService<IViewLocalizationRepository>() ??
				                                     new ViewLocalizationFileRepository());
			}
		}

		/// <summary>
		/// Translate a text prompt
		/// </summary>
		/// <param name="routeData">Used to lookup the controller location</param>
		/// <param name="text">Text to translate</param>
		/// <returns></returns>
		public virtual string Translate(RouteData routeData, string text)
		{
			if (routeData == null) throw new ArgumentNullException("routeData");
			if (string.IsNullOrEmpty(text))
				throw new ArgumentNullException("text");

			if (!Repository.Exists(CultureInfo.CurrentUICulture))
			{
				//use english as default
				var phrases = Repository.GetAllPrompts(CultureInfo.CurrentUICulture, new CultureInfo(1033), new SearchFilter());
				foreach (var phrase in phrases)
				{
					Repository.Save(CultureInfo.CurrentUICulture, phrase.ViewPath, phrase.TextName, phrase.TranslatedText);
				}
			}

			string textToSay = "";
			var uri = ViewPromptKey.GetViewPath(routeData);
			var id = new ViewPromptKey(uri, text);
			var prompt = Repository.GetPrompt(CultureInfo.CurrentUICulture, id);
			if (prompt == null)
				Repository.CreatePrompt(CultureInfo.CurrentUICulture, uri, text, "");
			else
				textToSay = prompt.TranslatedText;

			if (string.IsNullOrEmpty(textToSay))
				textToSay = CultureInfo.CurrentUICulture.Equals(DefaultCulture) ? text : string.Format("{1}:[{0}]", text, CultureInfo.CurrentUICulture);

			return textToSay;
		}
	}
}