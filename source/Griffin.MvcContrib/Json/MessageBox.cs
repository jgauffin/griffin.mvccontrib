namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Used to indicate that an message box (alert) should be displayed for the client
    /// </summary>
    /// <seealso cref="JsonResponse"/>
    public class MessageBox : IJsonResponseContent
    {
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        /// <param name="message">Message to show.</param>
        public MessageBox(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Gets message to show
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

    }
}