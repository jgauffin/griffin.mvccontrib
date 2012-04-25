using System.Web.Mvc;
using Newtonsoft.Json;

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
        /// <param name="success">Request was successful (false probably means that you want to return <see cref="ErrorMessage"/> or <see cref="ModelError"/>)</param>
        /// <param name="content">Content to return</param>
        /// <returns>Structured json</returns>
        public static ActionResult JsonResponse(this Controller controller, bool success, IJsonResponseContent content)
        {
            return new ContentResult
                       {
                           Content = JsonConvert.SerializeObject(new JsonResponse(success, content)),
                           ContentType = "application/json"
                       };
        }
    }
}