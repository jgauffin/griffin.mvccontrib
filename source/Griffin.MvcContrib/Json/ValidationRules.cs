using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    ///   Returns all validation rules for a model
    /// </summary>
    /// <remarks>
    ///   The rules (and their error messages) are formatted as the jQuery validation plugin wants them. So you can just pass them as options like this: <c>$('#yourform').validate(response.body)</c> .
    /// </remarks>
    [DataContract(Name = "validation-rules")]
    [XmlRoot("valdiation-rules")]
    public class ValidationRules : IJsonResponseContent
    {
        private readonly NameKeyValueList _messages = new NameKeyValueList();
        private readonly NameKeyValueList _rules = new NameKeyValueList();

        /// <summary>
        ///   Gets error messages
        /// </summary>
        /// <remarks>
        ///   the value is a formatted error message
        /// </remarks>
        [XmlElement("messages")]
        [DataMember(Name = "messages")]
        public NameKeyValueList Messages
        {
            get { return _messages; }
        }

        /// <summary>
        ///   Gets rules collection
        /// </summary>
        /// <remarks>
        ///   The rule name is the name which is used by the jQuery.validator plugin. Value are the options which is used by the rule. For instance the "max" rule takes "40" as the value if the max length is 40.
        /// </remarks>
        [XmlElement("rules")]
        [DataMember(Name = "rules")]
        public NameKeyValueList Rules
        {
            get { return _rules; }
        }
    }
}