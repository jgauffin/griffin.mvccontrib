namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    /// Used to locate files on disk for the <see cref="IViewFileProvider"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// public class DiskFileLocator : IViewFileProvider
    /// {
    ///     string _startUri;
    ///     string _diskRoot;
    /// 
    ///     public DiskFileLocator(string startUri, string diskRoot)
    ///     {
    ///         _diskRoot = diskRoot;
    ///         _startUri =  startUri;
    ///     }
    /// 
    ///     public string GetFullPath(string uri)
    ///     {
    ///          if (!uri.ToLower().StartWith(_startUri))
    ///              return null;
    /// 
    ///          var path = uri.Remove(0, _startUri.Length).Replace('/', '\\');
    ///          path = Path.Combine(_diskRoot, path);
    ///          if (File.Exists(path))
    ///              return path;
    ///     }
    /// }
    /// </code>
    /// </example>
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