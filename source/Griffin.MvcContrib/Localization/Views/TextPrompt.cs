using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Griffin.MvcContrib.Localization.Views
{
	/// <summary>
	/// A text that should be translated
	/// </summary>
	[Serializable]
	[DataContract]
	public class TextPrompt
	{
		public TextPrompt()
		{
			
		}
		public TextPrompt(TextPrompt source)
		{
			TextKey = source.TextKey;
			LocaleId = source.LocaleId;
			ActionName = source.ActionName;
			ControllerName = source.ControllerName;
			TextName = source.TextName;
			TranslatedText = source.TranslatedText;
		}

		/// <summary>
		/// Gets or sets locale id (refer to MSDN)
		/// </summary>
		[DataMember]
		public int LocaleId { get; set; }

		/// <summary>
		/// Gets or sets controller that the text is for
		/// </summary>
		[DataMember]
		public string ControllerName { get; set; }

		/// <summary>
		/// Gets or sets view name (unique in combination with controller name=
		/// </summary>
		[DataMember]
		public string ActionName { get; set; }

		/// <summary>
		/// Gets or sets the text to translate
		/// </summary>
		[DataMember]
		public string TextName { get; set; }

		/// <summary>
		/// Gets or sets translated text
		/// </summary>
		/// <value>Empty string if not translated</value>
		[DataMember]
		public string TranslatedText { get; set; }

		private string _textKey;

		/// <summary>
		/// Gets a key for the prompt.
		/// </summary>
		/// <remarks>The key must be unique in the current language but should be the
		/// same for all different languages.</remarks>
		[DataMember]
		public string TextKey
		{
			get
			{
				if (_textKey == null)
				{
					
				}
				return _textKey;
			}
			set { _textKey = value; }
		}

		/// <summary>
		/// Generate a key
		/// </summary>
		/// <param name="controllerName"></param>
		/// <param name="actionName"></param>
		/// <param name="textName"></param>
		/// <returns></returns>
		public static string CreateKey(string controllerName, string actionName, string textName)
		{
			var md5 = new MD5CryptoServiceProvider();
			var retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(controllerName + actionName + textName));
			var sb = new StringBuilder();
			for (var i = 0; i < retVal.Length; i++)
				sb.Append(retVal[i].ToString("x2"));
			return sb.ToString();			
		}
	}
}