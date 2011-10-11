using System.Web.Mvc;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Context used by <seealso cref="IFormItemAdapter"/>.
    /// </summary>
    public class FormItemAdapterContext : HtmlTagAdapterContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormItemAdapterContext"/> class.
        /// </summary>
        /// <param name="tagBuilder">Generated tag builder.</param>
        /// <param name="metadata">Model metadata.</param>
        public FormItemAdapterContext(NestedTagBuilder tagBuilder, ModelMetadata metadata)
            : base(tagBuilder)
        {
            Metadata = metadata;
        }

        /// <summary>
        /// Gets model metadata
        /// </summary>
        public ModelMetadata Metadata { get; private set; }
    }
}