using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Returns a model
    /// </summary>
    [XmlRoot("model-response")]
    [DataContract(Name = "model-response")]
    public class ModelResponse : IJsonResponseContent
    {
        private bool _manuallySet;
        private object _model;
        private string _modelName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelResponse"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public ModelResponse(object model)
        {
            Model = model;
        }

        /// <summary>
        /// Gets or sets model
        /// </summary>
        [XmlElement("model")]
        [DataMember(Name = "model")]
        public object Model
        {
            get { return _model; }
            private set
            {
                _model = value;
                if (!_manuallySet)
                {
                    _modelName = value.GetType().Name;
                }
            }
        }

        /// <summary>
        /// Gets or sets model
        /// </summary>
        [XmlElement("modelName")]
        [DataMember(Name = "modelName")]
        public string ModelName
        {
            get { return _modelName; }
            set
            {
                _manuallySet = true;
                _modelName = value;
            }
        }
    }
}