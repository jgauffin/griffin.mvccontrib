using System;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Uses reflection to format objects into select list items
    /// </summary>
    public class ReflectiveSelectItemFormatter : ISelectItemFormatter
    {
        private readonly string _textPropertyName;
        private readonly string _valuePropertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectiveSelectItemFormatter"/> class.
        /// </summary>
        /// <param name="textPropertyName">Name of the text/title property.</param>
        /// <param name="valuePropertyName">Name of the value property.</param>
        public ReflectiveSelectItemFormatter(string valuePropertyName, string textPropertyName)
        {
            _textPropertyName = textPropertyName;
            _valuePropertyName = valuePropertyName;
        }

        public SelectListItem Generate(object item)
        {
            if (_idGetter == null)
            {
                _idGetter= CreateDelegate(item.GetType(), _valuePropertyName);
                _titleGetter = CreateDelegate(item.GetType(), _textPropertyName);
            }

            return new SelectListItem
                       {
                           Text = _titleGetter(item).ToString(),
                           Value = _idGetter(item).ToString()
                       };
        }

        private static Func<object, object> CreateDelegate(Type itemType, string propertyName)
        {
            var propertyInfo = itemType.GetProperty(propertyName);
            if (propertyInfo == null)
                throw new InvalidOperationException("Failed to get a readable '" + propertyName + "' property for type " + itemType.FullName);

            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                throw new InvalidOperationException("Failed to get a readable '" + propertyName + "' property for type " + itemType.FullName);

            return instance => getMethod.Invoke(instance, null);
        }

        private Func<object, object> _idGetter;
        private Func<object, object> _titleGetter;

    }
}