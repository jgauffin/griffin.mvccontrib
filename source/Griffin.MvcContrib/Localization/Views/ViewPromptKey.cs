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
            _id = sb.ToString();
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

        #region IEquatable<ViewPromptKey> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ViewPromptKey other)
        {
            return other != null && other._id.Equals(_id);
        }

        #endregion

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
        /// Implements the operator ==.
        /// </summary>
        /// <param name="viewPromptKey">The view prompt key.</param>
        /// <param name="viewPromptKey2">The view prompt key2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ViewPromptKey viewPromptKey, ViewPromptKey viewPromptKey2)
        {
            if ((object)viewPromptKey == null || ((object)viewPromptKey2) == null)
                return Object.Equals(viewPromptKey, viewPromptKey2);

            return viewPromptKey.Equals(viewPromptKey2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="viewPromptKey">The view prompt key.</param>
        /// <param name="viewPromptKey2">The view prompt key2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ViewPromptKey viewPromptKey, ViewPromptKey viewPromptKey2)
        {
            if (viewPromptKey == null || viewPromptKey2 == null)
                return !Object.Equals(viewPromptKey, viewPromptKey2);

            return !(viewPromptKey.Equals(viewPromptKey2));
        }

        /// <summary>
        /// Generate a view path from route data 
        /// </summary>
        /// <param name="viewPath">VirtualPath to document (includes "~/Views and filename")</param>
        /// <param name="routeData">Route to create</param>
        /// <returns>Routed string</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetViewPath(string viewPath, RouteData routeData)
        {
            if (routeData == null) throw new ArgumentNullException("routeData");

            var path = viewPath.TrimStart('~');
            path = path.Remove(viewPath.LastIndexOf('.') - 1);
            path = path.Replace("/Views", "");
            return path;
        }

        /// <summary>
        /// Generate a view path from route data 
        /// </summary>
        /// <param name="routeData">Route to create</param>
        /// <returns>Routed string</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Obsolete("Use the version with the viewPath")]
        public static string GetViewPath(RouteData routeData)
        {
            if (routeData == null) throw new ArgumentNullException("routeData");

            var controllerName = routeData.GetRequiredString("Controller");
            var actionName = routeData.GetRequiredString("Action");
            var area = routeData.Values["area"] ?? routeData.DataTokens["area"];
            return area != null
                       ? string.Format("/{0}/{1}/{2}", area, controllerName, actionName)
                       : string.Format("/{0}/{1}", controllerName, actionName);
        }


        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ViewPromptKey)) return false;
            return Equals((ViewPromptKey)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (_id != null ? _id.GetHashCode() : 0);
        }
    }
}