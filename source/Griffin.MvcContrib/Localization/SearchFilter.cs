namespace Griffin.MvcContrib.Localization
{
    /// <summary>
    /// Used to filter localization prompts
    /// </summary>
    public class SearchFilter
    {
        /// <summary>
        /// Gets or sets beginning of the path to the prompts.
        /// </summary>
        /// <remarks>Path depends on the repository type. It might be namespace for type localization or View path for view localization</remarks>
        public string Path { get; set; }

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

        /// <summary>
        /// Gets or sets if only prompts which has not been translated should be shown.
        /// </summary>
        public bool OnlyNotTranslated { get; set; }
    }
}