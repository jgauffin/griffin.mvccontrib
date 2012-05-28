using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.MvcContrib.Html.Generators
{
    /// <summary>
    /// Used to generate a <see cref="NestedTagBuilder"/> for a specific HTML tag.
    /// </summary>
    public interface ITagBuilder
    {
        /// <summary>
        /// Generate a set of tags
        /// </summary>
        /// <param name="context">Context used during generation</param>
        /// <returns></returns>
        IEnumerable<NestedTagBuilder> Generate(ITagBuilderContext context);
    }
}
