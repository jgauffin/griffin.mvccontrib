using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.Localization.FlatFile
{
    /// <summary>
    /// all prompts for a language
    /// </summary>
    public class ViewPromptCollection : IEnumerable<ViewPrompt>
    {
        private readonly CultureInfo _culture;
        private readonly List<ViewPrompt> _prompts = new List<ViewPrompt>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPromptCollection"/> class.
        /// </summary>
        /// <param name="culture">The culture that the translation is for.</param>
        public ViewPromptCollection(CultureInfo culture)
        {
            _culture = culture;
        }

        /// <summary>
        /// Gets current culture
        /// </summary>
        public CultureInfo Culture
        {
            get { return _culture; }
        }

        #region IEnumerable<ViewPrompt> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ViewPrompt> GetEnumerator()
        {
            return _prompts.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Add a new prompt
        /// </summary>
        /// <param name="prompt">Found prompt</param>
        public void Add(ViewPrompt prompt)
        {
            if (prompt == null) throw new ArgumentNullException("prompt");
            _prompts.Add(prompt);
        }

        /// <summary>
        /// Get a specific prompt
        /// </summary>
        /// <param name="viewPath">Absolute paht to the view </param>
        /// <param name="key">Key to find</param>
        /// <returns>Translation if found; oterwise null.</returns>
        public string GetPrompt(string viewPath, ViewPromptKey key)
        {
            if (viewPath == null) throw new ArgumentNullException("viewPath");
            if (key == null) throw new ArgumentNullException("key");
            return
                _prompts.Where(p => p.ViewPath == viewPath && p.Key == key).Select(p => p.TranslatedText).FirstOrDefault
                    ();
        }

        /// <summary>
        /// Add a collection of prompts
        /// </summary>
        /// <param name="items">Prompts to add</param>
        public void AddRange(IEnumerable<ViewPrompt> items)
        {
            if (items == null) throw new ArgumentNullException("items");
            _prompts.AddRange(items);
        }

        /// <summary>
        /// Gets the specified prompt
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>Prompt if found; otherwise <c>null</c></returns>
        public ViewPrompt Get(ViewPromptKey id)
        {
            if (id == null) throw new ArgumentNullException("id");
            return _prompts.FirstOrDefault(p => p.Key == id);
        }

        /// <summary>
        /// delete prompt with the specified key
        /// </summary>
        /// <param name="key">key</param>
        public void Delete(ViewPromptKey key)
        {
            if (key == null) throw new ArgumentNullException("key");
            _prompts.RemoveAll(k => k.Key == key);
        }
    }
}