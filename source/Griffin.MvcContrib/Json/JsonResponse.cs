namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Response being sent back for JSON requests.
    /// </summary>
    /// <seealso cref="IJsonResponseContent"/>
    public class JsonResponse
    {
        private readonly IJsonResponseContent _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="content">Actual content.</param>
        public JsonResponse(IJsonResponseContent content)
        {
            _content = content;
        }

        /// <summary>
        /// Gets type of content which is sent back.
        /// </summary>
        public string ContentType
        {
            get { return _content.GetType().Name; }
        }

        /// <summary>
        /// Gets content
        /// </summary>
        public IJsonResponseContent Content
        {
            get { return _content; }
        }
    }
}
