namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    /// Used to locate files on disk for the <see cref="IViewFileProvider"/>.
    /// </summary>
    public interface IViewFileLocator
    {
        /// <summary>
        /// Get full path to a file
        /// </summary>
        /// <param name="uri">Requested uri</param>
        /// <returns>Full disk path if found; otherwise null.</returns>
        string GetFullPath(string uri);
    }
}