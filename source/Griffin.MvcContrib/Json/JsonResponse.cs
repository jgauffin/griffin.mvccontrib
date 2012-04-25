using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Response being sent back for JSON requests.
    /// </summary>
    /// <seealso cref="IJsonResponseContent"/>
    [DataContract]
    public class JsonResponse
    {
        private IJsonResponseContent _body;
        private bool _isContentTypeManuallySet;
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        /// <param name="success"> </param>
        /// <param name="body">Actual content.</param>
        public JsonResponse(bool success, IJsonResponseContent body)
        {
            if (body == null) throw new ArgumentNullException("body");
            _body = body;
            Success = success;
            SetContentType(body);
        }

        private void SetContentType(IJsonResponseContent body)
        {
            var attr =
                body.GetType().GetCustomAttributes(typeof(DataContractAttribute), true).Cast<DataContractAttribute>().
                    FirstOrDefault();
            ContentType = attr != null ? attr.Name : body.GetType().Name;
        }

        /// <summary>
        /// Gets or sets if the request was successful
        /// </summary>
        [XmlElement("success")]
        [DataMember(Name = "success", Order = 1)]
        public bool Success { get; set; }

        private string _contentType;

        /// <summary>
        /// Gets type of content which is sent back.
        /// </summary>
        [XmlElement("contentType")]
        [DataMember(Name = "contentType", Order = 2)]
        public string ContentType
        {
            get { return _contentType; }
            set
            {
                _contentType = value;
                _isContentTypeManuallySet = true;
            }
        }

        /// <summary>
        /// Gets content
        /// </summary>
        [XmlElement("body")]
        [DataMember(Name = "body", Order = 3)]
        public IJsonResponseContent Body
        {
            get { return _body; }
            set
            {
                _body = value;
                if (!_isContentTypeManuallySet)
                    SetContentType(value);
            }
        }
    }
}