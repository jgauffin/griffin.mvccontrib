using System.Web.Mvc;

namespace Griffin.MvcContrib.Html
{
    /// <summary>
    /// Takes any enumerable list and creates <see cref="SelectListItem"/>:s of them
    /// </summary>
    public interface ISelectItemFormatter
    {
        /// <summary>
        /// Generate a new select item from an object
        /// </summary>
        /// <param name="item">Object/Model item</param>
        /// <returns>Select item</returns>
        SelectListItem Generate(object item);
    }
}