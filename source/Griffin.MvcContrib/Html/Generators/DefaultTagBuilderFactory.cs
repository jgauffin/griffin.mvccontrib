using System;
using System.Collections.Generic;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Creates the tag builders which are used to format the tags.
    /// </summary>
    public class DefaultTagBuilderFactory : ITagBuilderFactory
    {
        private readonly Dictionary<string, ITagBuilderFactory> _tagBuilders =
            new Dictionary<string, ITagBuilderFactory>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTagBuilderFactory"/> class.
        /// </summary>
        public DefaultTagBuilderFactory()
        {
            _tagBuilders.Add("input.text", new DelegateTagBuilderFactory(name => new TextBoxGenerator()));
            _tagBuilders.Add("input.checkbox", new DelegateTagBuilderFactory(name => new CheckBoxGenerator()));
            _tagBuilders.Add("input.radio", new DelegateTagBuilderFactory(name => new RadioButtonGenerator()));
            _tagBuilders.Add("select", new DelegateTagBuilderFactory(name => new SelectGenerator()));
            _tagBuilders.Add("textarea", new DelegateTagBuilderFactory(name => new TextAreaGenerator()));
        }

        #region ITagBuilderFactory Members

        /// <summary>
        /// Create a new tag builder
        /// </summary>
        /// <param name="tagName">Name of HTML tag to generate</param>
        /// <returns>Tag builder</returns>
        /// <remarks>Should only be invoked for HTML tags which do not have a sub type.</remarks>
        public ITagBuilder Create(string tagName)
        {
            ITagBuilderFactory factory;
            if (_tagBuilders.TryGetValue(tagName, out factory))
                return factory.Create(tagName);

            return null;
        }

        /// <summary>
        /// Create a tag builder for a specific sub type
        /// </summary>
        /// <param name="tagName">Name of HTML tag to generate</param>
        /// <param name="type">Sub type (for instance the "type" attribute of INPUT tags)</param>
        /// <returns></returns>
        public ITagBuilder Create(string tagName, string type)
        {
            ITagBuilderFactory factory;
            if (!_tagBuilders.TryGetValue(tagName + "." + type, out factory))
                return factory.Create(tagName, type);

            if (!_tagBuilders.TryGetValue(tagName, out factory))
                return factory.Create(tagName, type);

            return null;
        }

        #endregion

        /// <summary>
        /// Map a factory to a tag
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="factory"></param>
        public void Map(string tagName, ITagBuilderFactory factory)
        {
            _tagBuilders[tagName] = factory;
        }

        /// <summary>
        /// Map a factory to a tag
        /// </summary>
        /// <param name="tagName">Name of HTML tag to generate</param>
        /// <param name="type">Sub type (for instance the "type" attribute of INPUT tags)</param>
        /// <param name="factory">Factory used to produce tag builders</param>
        public void Map(string tagName, string type, ITagBuilderFactory factory)
        {
            if (tagName == null) throw new ArgumentNullException("tagName");
            if (type == null) throw new ArgumentNullException("type");
            if (factory == null) throw new ArgumentNullException("factory");
            _tagBuilders[tagName] = factory;
        }

        #region Nested type: DelegateTagBuilderFactory

        private class DelegateTagBuilderFactory : ITagBuilderFactory
        {
            private readonly Func<string, ITagBuilder> _factoryMethod;

            public DelegateTagBuilderFactory(Func<string, ITagBuilder> factoryMethod)
            {
                _factoryMethod = factoryMethod;
            }

            #region ITagBuilderFactory Members

            public ITagBuilder Create(string tagName)
            {
                return _factoryMethod(tagName);
            }

            public ITagBuilder Create(string tagName, string type)
            {
                return _factoryMethod(tagName + "." + type);
            }

            #endregion
        }

        #endregion
    }
}