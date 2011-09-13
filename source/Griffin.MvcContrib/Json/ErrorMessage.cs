namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Display an error message for the user
    /// </summary>
    public class ErrorMessage : IJsonResponseContent
    {
        private readonly string _propertyName;
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ErrorMessage(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property that the error is for.</param>
        /// <param name="message">The message to display.</param>
        public ErrorMessage(string propertyName, string message)
        {
            _propertyName = propertyName;
            _message = message;
        }

        /// <summary>
        /// Gets property that the error is for, or <c>null</c> for a general error.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Gets message to display
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
    }
}