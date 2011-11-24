using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace Griffin.MvcContrib
{
	/// <summary>
	/// A provider which can let different providers serve the files and paths.
	/// </summary>
	/// <remarks>
	/// 
	/// </remarks>
	public class CompositeVirtualPathProvider : VirtualPathProvider
	{
		private List<VirtualPathProvider> _providers = new List<VirtualPathProvider>();
		private List<IVirtualFileProvider> _fileProviders = new List<IVirtualFileProvider>();

		public void Register(VirtualPathProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			_providers.Add(provider);
		}

		public override string CombineVirtualPaths(string basePath, string relativePath)
		{
			return base.CombineVirtualPaths(basePath, relativePath);
		}

		public override bool FileExists(string virtualPath)
		{
			return _fileProviders.Any(p => p.Exists(virtualPath)) || base.FileExists(virtualPath);
		}

		public override VirtualFile GetFile(string virtualPath)
		{
			var provider = _fileProviders.FirstOrDefault(p => p.Exists(virtualPath));
			return provider != null ? provider.Get(virtualPath) : base.GetFile(virtualPath);
		}

		public override VirtualDirectory GetDirectory(string virtualDir)
		{
			return base.GetDirectory(virtualDir);
		}
	}

	public interface IVirtualFileProvider
	{
		/// <summary>
		/// Checks if this provider has the specified file
		/// </summary>
		/// <param name="virtualFilePath">Virtual path to file, always starts with ~/</param>
		/// <returns>true if found; otherwise false.</returns>
		bool Exists(string virtualFilePath);

		VirtualFile Get(string virtualPath);
	}
}
