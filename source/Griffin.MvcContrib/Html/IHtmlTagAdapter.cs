namespace Griffin.MvcContrib.Html
{
	/// <summary>
	/// Adapter invoked for everything but FORM elements.
	/// </summary>
	public interface IHtmlTagAdapter
	{
		/// <summary>
		/// Process a tag
		/// </summary>
		/// <param name="context">Context with all html tag information</param>
		void Process(HtmlTagAdapterContext context);
	}
}