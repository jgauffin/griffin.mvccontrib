using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Griffin.MvcContrib.Localization.ValidationMessages
{
    /// <summary>
    /// Context information used to be able to identify and load the correct translation
    /// </summary>
    public class GetMessageContext : IGetMessageContext
    {
        private readonly ValidationAttribute _attribute;
        private readonly Type _containerType;
        private readonly CultureInfo _cultureInfo;
        private readonly string _propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetMessageContext"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="containerType">Model that the property is in.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="cultureInfo">Requested language.</param>
        public GetMessageContext(ValidationAttribute attribute, Type containerType, string propertyName,
                                 CultureInfo cultureInfo)
        {
            if (attribute == null) throw new ArgumentNullException("attribute");
            if (containerType == null) throw new ArgumentNullException("containerType");
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (cultureInfo == null) throw new ArgumentNullException("cultureInfo");
            _attribute = attribute;
            _containerType = containerType;
            _propertyName = propertyName;
            _cultureInfo = cultureInfo;
        }

        #region IGetMessageContext Members

        /// <summary>
        /// Gets attribute to get message for.
        /// </summary>
        public ValidationAttribute Attribute
        {
            get { return _attribute; }
        }

        /// <summary>
        /// Gets model that the property exists in
        /// </summary>
        public Type ContainerType
        {
            get { return _containerType; }
        }

        /// <summary>
        /// Gets name of the target property
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Gets requested language
        /// </summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
        }

        #endregion
    }
}