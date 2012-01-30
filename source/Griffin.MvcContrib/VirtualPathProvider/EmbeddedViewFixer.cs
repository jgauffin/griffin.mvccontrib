using System.IO;
using System.Text;

namespace Griffin.MvcContrib.VirtualPathProvider
{
    /// <summary>
    ///   Adds default usings, sets an inherits clause and specifies the layout name
    /// </summary>
    public class EmbeddedViewFixer : IEmbeddedViewFixer
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="EmbeddedViewFixer" /> class.
        /// </summary>
        public EmbeddedViewFixer()
        {
            WebViewPageClassName = "Griffin.MvcContrib.GriffinWebViewPage";
            LayoutPath = "~/Views/Shared/_Layout.5.0.cshtml";
        }

        /// <summary>
        ///   Base view class to inherit.
        /// </summary>
        /// <example>
        ///   <code>GriffinVirtualPathProvider.Current.LayoutPath = "Griffin.MvcContrib.GriffinWebViewPage";</code>
        /// </example>
        /// <value> Default is Griffin.MvcContrib.GriffinWebViewPage </value>
        public string WebViewPageClassName { get; set; }

        /// <summary>
        ///   Gets or sets relative path to the layout file to use
        /// </summary>
        /// <example>
        ///   <code>GriffinVirtualPathProvider.Current.LayoutPath = "~/Views/Shared/_Layout.cshtml";</code>
        /// </example>
        /// <value> Default is "~/Views/Shared/_Layout.cshtml" </value>
        public string LayoutPath { get; set; }

        #region IEmbeddedViewFixer Members

        /// <summary>
        ///   Modify the view
        /// </summary>
        /// <param name="virtualPath"> Path to view </param>
        /// <param name="stream"> Stream containing the original view </param>
        /// <returns> Stream with modified contents </returns>
        public Stream CorrectView(string virtualPath, Stream stream)
        {
            var reader = new StreamReader(stream, Encoding.UTF8);
            var view = reader.ReadToEnd();
            stream.Close();
            var ourStream = new MemoryStream();
            var writer = new StreamWriter(ourStream, Encoding.UTF8);

            var modelString = "";
            var modelPos = view.IndexOf("@model");
            if (modelPos != -1)
            {
                writer.Write(view.Substring(0, modelPos));
                var modelEndPos = view.IndexOfAny(new[] {'\r', '\n'}, modelPos);
                modelString = view.Substring(modelPos, modelEndPos - modelPos);
                view = view.Remove(0, modelEndPos);
            }

            writer.WriteLine("@using System.Web.Mvc");
            writer.WriteLine("@using System.Web.Mvc.Ajax");
            writer.WriteLine("@using System.Web.Mvc.Html");
            writer.WriteLine("@using System.Web.Routing");

            var basePrefix = "@inherits " + WebViewPageClassName;

            if (virtualPath.ToLower().Contains("__viewstart"))
                writer.WriteLine("@inherits System.Web.WebPages.StartPage");
            else if (modelString == "@model object")
                writer.WriteLine(basePrefix + "<dynamic>");
            else if (!string.IsNullOrEmpty(modelString))
                writer.WriteLine(basePrefix + "<" + modelString.Substring(7) + ">");
            else
                writer.WriteLine(basePrefix);

            writer.WriteLine(string.Format("@{{ Layout = \"{0}\"; }}", LayoutPath));
            writer.Write(view);
            writer.Flush();
            ourStream.Position = 0;
            return ourStream;
        }

        #endregion
    }
}