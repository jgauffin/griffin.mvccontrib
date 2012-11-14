using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    ///   Maps files on the hard drive.
    /// </summary>
    public class DiskFileLocator : IViewFileLocator
    {
        private readonly List<Mapping> _mappings = new List<Mapping>();
        private IEnumerable<string> _allowedFileExtensions;

        #region IViewFileLocator Members

        /// <summary>
        ///   Get full path to a file
        /// </summary>
        /// <param name="uri"> Requested uri </param>
        /// <returns> Full disk path if found; otherwise null. </returns>
        public string GetFullPath(string uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            uri = VirtualPathUtility.ToAbsolute(uri);
            var mapping = _mappings.OrderByDescending(x => x.Uri.Length).FirstOrDefault(x => uri.StartsWith(x.Uri));
            if (mapping == null)
                return null;

            var path = uri.Remove(0, mapping.Uri.Length).Replace('/', '\\');
            path = Path.Combine(mapping.DiskRoot, path);

            if (!IsFileAllowed(path))
                return null;

            return File.Exists(path) ? path : null;
        }


        /// <summary>
        /// Set extensions that are allowed to be scanned.
        /// </summary>
        /// <param name="fileExtensions">File extensions without the dot.</param>
        public void SetAllowedExtensions(IEnumerable<string> fileExtensions)
        {
            if (fileExtensions == null) throw new ArgumentNullException("fileExtensions");
            _allowedFileExtensions = fileExtensions;
        }

        #endregion

        /// <summary>
        /// determins if the found embedded file might be mapped and provided.
        /// </summary>
        /// <param name="fullPath">Full path to the file</param>
        /// <returns><c>true</c> if the file is allowed; otherwise <c>false</c>.</returns>
        protected virtual bool IsFileAllowed(string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException("fullPath");

            var extension = fullPath.Substring(fullPath.LastIndexOf('.') + 1);
            return _allowedFileExtensions.Any(x => x == extension);
        }

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
                    Uri = VirtualPathUtility.ToAbsolute(rootUri),
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