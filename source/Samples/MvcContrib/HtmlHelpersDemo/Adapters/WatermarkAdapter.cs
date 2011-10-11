using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Griffin.MvcContrib.Html;

namespace HtmlHelpersDemo.Adapters
{
    public class WatermarkAdapter : IFormItemAdapter
    {
        /// <summary>
        /// Process a tag
        /// </summary>
        /// <param name="context">Context with all html tag information</param>
        public void Process(FormItemAdapterContext context)
        {
            // there is a "Watermark" metadata but it cannot currently be set.
            // unless you are using a custom metadata provider like the one in the localization demo.
            if (string.IsNullOrEmpty(context.Metadata.Description) || !context.TagBuilder.Attributes.ContainsKey("class") || !context.TagBuilder.Attributes["class"].Contains("watermark"))
                return;

            context.TagBuilder.MergeAttribute("title", "Watermark:" + context.Metadata.Description);
        }
    }
}