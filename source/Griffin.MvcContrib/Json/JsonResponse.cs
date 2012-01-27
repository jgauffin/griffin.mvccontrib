namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Response being sent back for JSON requests.
    /// </summary>
    /// <seealso cref="IJsonResponseContent"/>
    public class JsonResponse
    {
        private readonly IJsonResponseContent _body;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="body">Actual content.</param>
        public JsonResponse(IJsonResponseContent body)
        {
            _body = body;
        }

        /// <summary>
        /// Gets type of content which is sent back.
        /// </summary>
        public string ContentType
        {
            get { return _body.GetType().Name; }
        }

        /// <summary>
        /// Gets content
        /// </summary>
        public IJsonResponseContent Body
        {
            get { return _body; }
        }
    }
}