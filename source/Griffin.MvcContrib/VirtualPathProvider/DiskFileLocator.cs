using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    ///   Maps files on the hard drive.
    /// </summary>
    public class DiskFileLocator : IViewFileLocator
    {
        private readonly List<Mapping> _mappings = new List<Mapping>();

        #region IViewFileLocator Members

        /// <summary>
        ///   Get full path to a file
        /// </summary>
        /// <param name="uri"> Requested uri </param>
        /// <returns> Full disk path if found; otherwise null. </returns>
        public string GetFullPath(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            var mapping = _mappings.OrderByDescending(x => x.Uri.Length).FirstOrDefault(x => x.Uri.StartsWith(uri));
            if (mapping == null)
                return null;

            var path = uri.Remove(0, mapping.Uri.Length).Replace('/', '\\');
            path = Path.Combine(mapping.DiskRoot, path);
            return File.Exists(path) ? path : null;
        }

        #endregion

        /// <summary>
        ///   Adds the specified root URI.
        /// </summary>
        /// <param name="rootUri"> Root uri (must be the first part of the URI for this class to handle the request). </param>
        /// <param name="rootPath"> Location on disk drive which corresponds to the root uri. </param>
        /// <example>
        ///   <code>fileLocator.Add("/files/", @"C:\inetpub\wwwroot\mysite\public");</code>
        /// </example>
        public void Add(string rootUri, string rootPath)
        {
            if (rootUri == null) throw new ArgumentNullException("rootUri");
            if (rootPath == null) throw new ArgumentNullException("rootPath");
            _mappings.Add(new Mapping
                              {
                                  Uri = rootUri,
                                  DiskRoot = rootPath
                              });
        }

        #region Nested type: Mapping

        private class Mapping
        {
            public string DiskRoot { get; set; }
            public string Uri { get; set; }
        }

        #endregion
    }
}