using System;

namespace Griffin.MvcContrib.RavenDb
{
    /// <summary>
    /// Can be used to find all services in the framework which should be registered.
    /// </summary>
    /// <remarks>Not used yet.</remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterServiceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterServiceAttribute"/> class.
        /// </summary>
        /// <param name="feature">The category.</param>
        public RegisterServiceAttribute(string feature)
        {
            Feature = feature;
        }

        /// <summary>
        /// Gets feature that the service is for.
        /// </summary>
        public string Feature { get; set; }
    }
}