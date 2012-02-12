namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Used to filter localization prompts
    /// </summary>
    public class SearchFilter
    {
        /// <summary>
        /// Gets or sets start of text prompt
        /// </summary>
        public string TextFilter { get; set; }

        /// <summary>
        /// Gets or sets page to get
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets number of items per page.
        /// </summary>
        public int PageSize { get; set; }
    }
}