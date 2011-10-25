using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Griffin.MvcContrib.Localization.Views
{
	/// <summary>
	/// 
	/// </summary>
	public class TextPromptCollection : IEnumerable<TextPrompt>
	{
		private readonly CultureInfo _culture;
		private List<TextPrompt> _prompts = new List<TextPrompt>();

		public TextPromptCollection(CultureInfo culture)
		{
			_culture = culture;
		}

		public CultureInfo Culture
		{
			get { return _culture; }
		}

		public void Add(TextPrompt prompt)
		{
			_prompts.Add(prompt);
		}

		public string GetPrompt(string controllerName, string actionName, string text)
		{
			return _prompts.Where(p => p.TextName == text).Select(p => p.TranslatedText).FirstOrDefault();
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

		public void AddRange(IEnumerable<TextPrompt> items)
		{
			_prompts.AddRange(items);
		}

		public TextPrompt Get(string id)
		{
			return _prompts.Where(p => p.TextKey == id).FirstOrDefault();
		}
	}
}