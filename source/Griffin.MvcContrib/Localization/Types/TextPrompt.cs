using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Griffin.MvcContrib.Localization.Types
{
	/// <summary>
	/// Used to store translated prompts.
	/// </summary>
	[DataContract]
	public class TextPrompt
	{
		public TextPrompt()
		{
			UpdatedAt = DateTime.Now;
		}
		public TextPrompt(TextPrompt source)
		{
			UpdatedAt = DateTime.Now;
			LocaleId = source.LocaleId;
			this.Subject = source.Subject;
			this.TextKey = source.TextKey;
			TextName = source.TextName;
			TranslatedText = source.TranslatedText;
		}

		/// <summary>
		/// Gets or sets target class
		/// </summary>
		/// <remarks>Might be a enum, validation attribute or model type</remarks>
		public Type Subject { get; set; }

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
		public string TextKey { get; set; }

		/// <summary>
		/// Gets or sets user id (using current identity <see cref="HttpContext.User"/>).
		/// </summary>
		public string UpdatedBy { get; set; }

		/// <summary>
		/// Generate a key
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="actionName"></param>
		/// <param name="textName"></param>
		/// <returns></returns>
		public static string CreateKey(Type subject, string name)
		{
			var md5 = new MD5CryptoServiceProvider();
			var retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(subject.FullName + name));
			var sb = new StringBuilder();
			for (var i = 0; i < retVal.Length; i++)
				sb.Append(retVal[i].ToString("x2"));
			return sb.ToString();
		}

	}
}
