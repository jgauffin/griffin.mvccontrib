namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Adapters can modify the generated HTML tags.
    /// </summary>
    /// <remarks>
    /// Use to extend the tags or adapt them to suit your own HTML design.
    /// </remarks>
    public interface IFormItemAdapter
    {
        /// <summary>
        /// Process a tag
        /// </summary>
        /// <param name="context">Context with all html tag information</param>
        void Process(FormItemAdapterContext context);
    }
}