using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Send back a simple message and let the handler decide what to do with it
    /// </summary>
    [DataContract(Name = "message")]
    [XmlRoot("message")]
    public class SimpleMessage : IJsonResponseContent
    {
        /// <summary>
        /// Gets or sets the message
        /// </summary>
        [XmlElement("value")]
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
