using System.Web.Caching;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    /// Use this implementation to indicate that no cache should be used.
    /// </summary>
    public class NoCache : CacheDependency
    {
        private static readonly NoCache _instance = new NoCache();

        /// <summary>
        /// Prevents a default instance of the <see cref="NoCache"/> class from being created.
        /// </summary>
        private NoCache()
        {
        }

        /// <summary>
        /// Gets current instance.
        /// </summary>
        public static NoCache Instance
        {
            get { return _instance; }
        }
    }
}