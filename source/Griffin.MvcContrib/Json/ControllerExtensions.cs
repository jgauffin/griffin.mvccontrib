using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Extension methods for working with structured JSON
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Return a response.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static JsonResult JsonResponse( this Controller controller, IJsonResponseContent content)
        {
            return new JsonResult{Data = content};
        }

        public static JsonResult JsonResponse(this Controller controller, IJsonResponseContent content, JsonRequestBehavior requestBehavior)
        {
            return new JsonResult {Data = content, JsonRequestBehavior = requestBehavior};
        }
    }
}
