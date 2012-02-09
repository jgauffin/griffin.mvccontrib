using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Routing;

namespace Griffin.MvcContrib.Localization.Views
{
	/// <summary>
	/// View prompt key.
	/// </summary>
	/// <remarks>The key is only unique for the current language only. This is a requirement
	/// to be able to translate prompts between languages</remarks>
	[DataContract]
	public class ViewPromptKey : IEquatable<ViewPromptKey>
	{
        [DataMember]
		private readonly string _id;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewPromptKey"/> class.
		/// </summary>
		/// <param name="viewPath">The view path (normally <see cref="Uri.AbsolutePath"/>*).</param>
		/// <param name="textName">Name of the text.</param>
		public ViewPromptKey(string viewPath, string textName)
		{
			if (viewPath == null) throw new ArgumentNullException("viewPath");
			if (textName == null) throw new ArgumentNullException("textName");

			var md5 = new MD5CryptoServiceProvider();
			var retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(viewPath + textName));
			var sb = new StringBuilder();
			for (var i = 0; i < retVal.Length; i++)
				sb.Append(retVal[i].ToString("x2"));
			_id= sb.ToString();			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewPromptKey"/> class.
		/// </summary>
		/// <param name="md5Hash">Already computed hash.</param>
		public ViewPromptKey(string md5Hash)
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
		public bool Equals(ViewPromptKey other)
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

		/// <summary>
		/// Generate a view path from route data 
		/// </summary>
		/// <param name="routeData">Route to create</param>
		/// <returns>Routed string</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string GetViewPath(RouteData routeData)
		{
			if (routeData == null) throw new ArgumentNullException("routeData");

			var controllerName = routeData.GetRequiredString("Controller");
			var actionName = routeData.GetRequiredString("Action");
			var area = routeData.Values["area"];
			return area != null
						? string.Format("/{0}/{1}/{2}", area, controllerName, actionName)
						: string.Format("/{0}/{1}", controllerName, actionName);
		}
	}
}