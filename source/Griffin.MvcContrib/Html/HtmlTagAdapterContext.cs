namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Context used by <seealso cref="IHtmlTagAdapter"/>.
    /// </summary>
    public class HtmlTagAdapterContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormItemAdapterContext"/> class.
        /// </summary>
        /// <param name="tagBuilder">Generated tag builder.</param>
        public HtmlTagAdapterContext(NestedTagBuilder tagBuilder)
        {
            TagBuilder = tagBuilder;
        }

        /// <summary>
        /// Gets generated tag
        /// </summary>
        /// <remarks>Either adapt the tag and it's content or replace it with a new one.</remarks>
        public NestedTagBuilder TagBuilder { get; set; }
    }
}