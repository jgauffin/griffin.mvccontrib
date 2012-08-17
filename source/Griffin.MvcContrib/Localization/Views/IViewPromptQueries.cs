using System.Globalization;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    /// Queries used to find view texts
    /// </summary>
    public interface IViewPromptQueries
    {
        /// <summary>
        /// Find items
        /// </summary>
        /// <param name="culture">Culture to get prompsts for</param>
        /// <param name="text">Search view path and name after this string</param>
        /// <param name="constraints">Used to limit the search result</param>
        /// <returns>Matching items</returns>
        IViewPromptResult FindText(CultureInfo culture, string text, QueryConstraints constraints);

        /// <summary>
        /// Find items
        /// </summary>
        /// <param name="culture">Culture to get prompsts for</param>
        /// <param name="constraints">Used to limit the search result</param>
        /// <returns>Matching items</returns>
        IViewPromptResult FindAll(CultureInfo culture, QueryConstraints constraints);

        /// <summary>
        /// Find items which has not been translated into the specified language
        /// </summary>
        /// <param name="culture">Culture to get prompsts for</param>
        /// <param name="constraints">Used to limit the search result</param>
        /// <returns>Matching items</returns>
        IViewPromptResult FindNotTranslated(CultureInfo culture, QueryConstraints constraints);
    }
}