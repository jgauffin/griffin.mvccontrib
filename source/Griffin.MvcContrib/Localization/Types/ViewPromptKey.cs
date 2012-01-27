using System;
using System.Security.Cryptography;
using System.Text;
using Griffin.MvcContrib.Localization.Views;

namespace Griffin.MvcContrib.Localization.Types
{
	/// <summary>
	/// Type prompt key.
	/// </summary>
	/// <remarks>The key is only unique for the current language only. This is a requirement
	/// to be able to translate prompts between languages</remarks>
	public class TypePromptKey : IEquatable<TypePromptKey>
	{
		private readonly string _id;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewPromptKey"/> class.
		/// </summary>
		/// <param name="type">Type that get a localization.</param>
		/// <param name="name">Property name (and metadata name prefixed with underscore).</param>
		public TypePromptKey(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException("type");
			if (name == null) throw new ArgumentNullException("name");

			var md5 = new MD5CryptoServiceProvider();
			var retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(type.FullName + name));
			var sb = new StringBuilder();
			for (var i = 0; i < retVal.Length; i++)
				sb.Append(retVal[i].ToString("x2"));
			_id = sb.ToString();			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewPromptKey"/> class.
		/// </summary>
		/// <param name="md5Hash">Already computed hash.</param>
		public TypePromptKey(string md5Hash)
		{
			if (md5Hash == null) throw new ArgumentNullException("md5Hash");
			_id = md5Hash;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(TypePromptKey other)
		{
			return other._id.Equals(_id);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return _id;
		}
	}
}