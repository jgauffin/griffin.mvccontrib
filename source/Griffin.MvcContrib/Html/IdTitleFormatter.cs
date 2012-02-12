namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Maps to "Id" and "Title" properties
    /// </summary>
    public class IdTitleFormatter : ReflectiveSelectItemFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdTitleFormatter"/> class.
        /// </summary>
        public IdTitleFormatter()
            : base("Id", "Title")
        {
        }
    }
}