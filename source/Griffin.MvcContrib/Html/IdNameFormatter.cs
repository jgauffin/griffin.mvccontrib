namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Maps to "Id" and "Name" properties
    /// </summary>
    public class IdNameFormatter : ReflectiveSelectItemFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdNameFormatter"/> class.
        /// </summary>
        public IdNameFormatter()
            : base("Id", "Name")
        {
        }
    }
}