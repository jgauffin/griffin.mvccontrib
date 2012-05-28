using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Display an error message for the user
    /// </summary>
    [XmlRoot("error")]
    [DataContract(Name = "error", Namespace = "")]
    public class ErrorMessage : IJsonResponseContent
    {
        private string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ErrorMessage(string message)
        {
            _message = message;
        }

        /// <summary>
        /// Gets the message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get { return _message; } protected set { _message = value; }}

        /// <summary>
        /// Gets message to display
        /// </summary>
        public override string  ToString()
        {
            return _message;
        }
    }
}