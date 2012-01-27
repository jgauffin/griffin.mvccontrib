using System;
using System.Runtime.Serialization;
using System.Web;

namespace Griffin.MvcContrib.Localization.Types
{
	/// <summary>
	/// Used to store translated prompts.
	/// </summary>
	[DataContract]
	public class TextPrompt : IEquatable<TextPrompt>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextPrompt"/> class.
		/// </summary>
		public TextPrompt()
		{
			UpdatedAt = DateTime.Now;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TextPrompt"/> class.
		/// </summary>
		/// <param name="localeId">New locale </param>
		/// <param name="source">Copies all but translated text.</param>
		public TextPrompt(int localeId, TextPrompt source)
		{
			if (source == null) throw new ArgumentNullException("source");

			UpdatedAt = DateTime.Now;
			LocaleId = source.LocaleId;
			Subject = source.Subject;
			Key = source.Key;
			TextName = source.TextName;
			TranslatedText = "";
		}


		/// <summary>
		/// Gets or sets target class
		/// </summary>
		/// <remarks>Might be a enum, validation attribute or model type</remarks>
		public Type Subject { get; set; }

		/// <summary>
		/// Gets or sets assembly qualified name
		/// </summary>
		[DataMember]
		public string SubjectTypeName
		{
			get { return Subject.AssemblyQualifiedName; }
			set { Subject = Type.GetType(value); }
		}

		/// <summary>
		/// Gets or sets text name
		/// </summary>
		/// <remarks>Might be a property name, an enum value name or just empty (actual type is being translated)</remarks>
		[DataMember]
		public string TextName { get; set; }

		/// <summary>
		/// Gets actual translation
		/// </summary>
		[DataMember]
		public string TranslatedText { get; set; }

		/// <summary>
		/// Gets when the prompt was updated.
		/// </summary>
		[DataMember]
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// Ges or sets LCID identifier.
		/// </summary>
		[DataMember]
		public int LocaleId { get; set; }

		/// <summary>
		/// Gets or sets ID is unique for the current entry (in the current language)
		/// </summary>
		/// <remarks>The id should be the same in all languages</remarks>
		[DataMember]
		public TypePromptKey Key { get; set; }

		/// <summary>
		/// Gets or sets user id (using current identity <see cref="HttpContext.User"/>).
		/// </summary>
		public string UpdatedBy { get; set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type. (language not taken into account)
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(TextPrompt other)
		{
			return other.Key.Equals(Key);
		}
	}
}