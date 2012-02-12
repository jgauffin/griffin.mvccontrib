using System.Collections;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Context used when generating select lists
    /// </summary>
    public class SelectContext : GeneratorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectContext"/> class.
        /// </summary>
        /// <param name="context">Base context.</param>
        /// <param name="formatter"><see cref="Formatter"/></param>
        /// <param name="listItems"><seealso cref="ListItems"/></param>
        public SelectContext(GeneratorContext context, ISelectItemFormatter formatter, IEnumerable listItems)
            : base(context)
        {
            Formatter = formatter;
            ListItems = listItems;
        }

        /// <summary>
        /// Gets formatter used to convert model/object into a select item
        /// </summary>
        public ISelectItemFormatter Formatter { get; private set; }

        /// <summary>
        /// Gets a list of items that a SELECT list should be generated from
        /// </summary>
        public IEnumerable ListItems { get; private set; }
    }
}