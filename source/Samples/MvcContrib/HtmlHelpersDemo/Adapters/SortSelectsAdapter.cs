using System.Linq;
using Griffin.MvcContrib.Html;

namespace HtmlHelpersDemo.Adapters
{

    /// <summary>
    /// will automatically sort all SELECT tags by their names
    /// </summary>
    public class SortSelectsAdapter : IFormItemAdapter
    {
        /// <summary>
        /// Process a tag
        /// </summary>
        /// <param name="context">Context with all html tag information</param>
        public void Process(FormItemAdapterContext context)
        {
            if (!context.TagBuilder.Attributes.ContainsKey("class") 
                || !context.TagBuilder.Attributes["class"].Contains("sortMe")) 
                return;

            // remove the sortMe
            var value = context.TagBuilder.Attributes["class"].Replace("sortMe", "");
            context.TagBuilder.Attributes["class"] = value;

            var ordered = context.TagBuilder.Children.OrderBy(t => t.InnerHtml).ToList();
            context.TagBuilder.RemoveChildren();
            context.TagBuilder.AddChildren(ordered);
        }
    }
}