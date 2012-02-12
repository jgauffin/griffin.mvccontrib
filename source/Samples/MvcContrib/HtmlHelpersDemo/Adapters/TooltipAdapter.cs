using Griffin.MvcContrib.Html;

namespace HtmlHelpersDemo.Adapters
{
    /// <summary>
    /// Adds a title attribute which is picked up in the application.js script
    /// </summary>
    public class TooltipAdapter : IFormItemAdapter
    {
        #region IFormItemAdapter Members

        /// <summary>
        /// Process a tag
        /// </summary>
        /// <param name="context">Context with all html tag information</param>
        public void Process(FormItemAdapterContext context)
        {
            if (string.IsNullOrEmpty(context.Metadata.Description) || context.TagBuilder.Attributes.ContainsKey("title"))
                return;

            context.TagBuilder.MergeAttribute("title", context.Metadata.Description);
        }

        #endregion
    }
}