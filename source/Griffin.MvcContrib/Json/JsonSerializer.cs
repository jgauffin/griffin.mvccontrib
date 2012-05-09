using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Used to serialize the objects.
    /// </summary>
    /// <remarks>
    /// RavenDB currently only supports Newtonsoft.Json v4.0.8 which is kind of old when 4.5.x exists. I therefore
    /// decided to remove the Newtonsoft.Json dependency since I do not want to force the non-raven users to use
    /// an old version of Newtonsoft.Json. You can create the support yourself by using the code sample below (after installing
    /// the Newtonsoft.Json nuget package).
    /// </remarks>
    /// <example>
    /// <code>
    /// public class NewtonsoftSerializer : JsonSerializer
    /// {
    ///     public override string Serialize(object value)
    ///     {
    ///         return JsonConvert.SerializeObject(value);
    ///     }
    /// }
    /// JsonSerializer.Assign(new NewtonsoftSerializer());
    /// </code>
    /// </example>
    public abstract class JsonSerializer
    {
        private static JsonSerializer _instance;

        /// <summary>
        /// Assign a serializer implementation.
        /// </summary>
        /// <param name="current"></param>
        public static void Assign(JsonSerializer current)
        {
            if (current == null) throw new ArgumentNullException("current");
            if (current.GetType() == typeof(JsonSerializer))
                throw new InvalidOperationException("The default implementation do not serialize anything.");

            _instance = current;
        }

        /// <summary>
        /// Gets current instance
        /// </summary>
        public static JsonSerializer Current
        {
            get
            {
                if (_instance == null)
                    throw new NotSupportedException("You need to configure a JSON Serializer. Read the docs for the class Griffin.MvcContrib.Json.JsonSerializer");
                return _instance;
            }
        }

        /// <summary>
        /// Serialize an object.
        /// </summary>
        /// <param name="value">value to serialize</param>
        /// <returns>A string</returns>
        public abstract string Serialize(object value);
    }

}
