using System;

namespace Griffin.MvcContrib.Localization.Views
{
    /// <summary>
    /// Used to limit the search result
    /// </summary>
    public class QueryConstraints
    {
        /// <summary>
        /// Gets zero based index for paging
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Gets page to get
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets property to sort by
        /// </summary>
        public string SortPropertyName { get; private set; }

        /// <summary>
        /// Gets sort order
        /// </summary>
        public SortDirection SortOrder { get; private set; }

        /// <summary>
        /// Sort the returned items
        /// </summary>
        /// <param name="propertyName">Property to sort by</param>
        /// <param name="direction">Sort direction</param>
        /// <returns>current instance</returns>
        public QueryConstraints Sort(string propertyName, SortDirection direction)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            SortPropertyName = propertyName;
            SortOrder = direction;
            return this;
        }

        /// <summary>
        /// Page the result
        /// </summary>
        /// <param name="pageNumber">Page to get, one based index.</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>this</returns>
        public QueryConstraints Page(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageNumber > 1000)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "Must be between 1 and 1000");
            if (pageNumber < 1 || pageNumber > 1000)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "Must be between 1 and 1000");

            PageNumber = pageNumber;
            PageSize = pageSize;
            return this;
        }
    }
}