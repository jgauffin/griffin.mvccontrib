using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Griffin.MvcContrib.Localization.Types
{
	/// <summary>
	/// All prompts for a specific language
	/// </summary>
	public class TextPromptCollection : IEnumerable<TextPrompt>
	{
		private readonly CultureInfo _culture;
		private readonly List<TextPrompt> _prompts = new List<TextPrompt>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TextPromptCollection"/> class.
		/// </summary>
		/// <param name="culture">The culture that all prompts are for.</param>
		public TextPromptCollection(CultureInfo culture)
		{
			_culture = culture;
		}

		/// <summary>
		/// Gets culture that the prompt is for
		/// </summary>
		public CultureInfo Culture
		{
			get { return _culture; }
		}

		/// <summary>
		/// Add a new prompt
		/// </summary>
		/// <param name="prompt">Found prompt</param>
		public void Add(TextPrompt prompt)
		{
			if (prompt == null) throw new ArgumentNullException("prompt");
			if (prompt.LocaleId != Culture.LCID)
				throw new ArgumentException("Prompt is for " + prompt.LocaleId + ", our language is " + Culture);
			
			_prompts.Add(prompt);
		}

		/// <summary>
		/// Translate a prompt if found
		/// </summary>
		/// <param name="key">Prompt to translate</param>
		/// <returns>Translation if found; otherwise null.</returns>
		public string Translate(TypePromptKey key)
		{
			return _prompts.Where(p => p.Key == key).Select(p => p.TranslatedText).FirstOrDefault();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<TextPrompt> GetEnumerator()
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

		/// <summary>
		/// Add a collection of prompts.
		/// </summary>
		/// <param name="items"></param>
		public void AddRange(IEnumerable<TextPrompt> items)
		{
			if (items == null) throw new ArgumentNullException("items");
			_prompts.AddRange(items);
		}

		/// <summary>
		/// Get a prompt
		/// </summary>
		/// <param name="id">Prompt id</param>
		/// <returns>prompt if found; otherwise null</returns>
		public TextPrompt Get(TypePromptKey id)
		{
			if (id == null) throw new ArgumentNullException("id");
			return _prompts.FirstOrDefault(p => p.Key == id);
		}
	}
}
