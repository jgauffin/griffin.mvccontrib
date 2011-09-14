using System.Web.Mvc;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Extension methods for working with structured JSON
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Return a structured JSON response.
        /// </summary>
        /// <param name="controller">Controller returning the result</param>
        /// <param name="content">Content to return</param>
        /// <returns>Structured json</returns>
        public static JsonResult JsonResponse(this Controller controller, IJsonResponseContent content)
        {
            return new JsonResult {Data = content};
        }

        /// <summary>
        /// Return a structured JSON response.
        /// </summary>
        /// <param name="controller">Controller returning the result</param>
        /// <param name="content">Content to return</param>
        /// <param name="requestBehavior">How HTTP Requests should be treaded.</param>
        /// <returns>Structured json</returns>
        public static JsonResult JsonResponse(this Controller controller, IJsonResponseContent content,
                                              JsonRequestBehavior requestBehavior)
        {
            return new JsonResult {Data = content, JsonRequestBehavior = requestBehavior};
        }
    }
}