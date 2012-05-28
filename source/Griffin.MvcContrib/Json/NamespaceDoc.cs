using System.Runtime.CompilerServices;

namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// Classes making it easier to handle JSON in MVC
    /// </summary>
    /// <remarks>
    /// <para>
    /// Most of the classes are used to be able return useful results from the controller to the jQuery scripts at client-side. Most
    /// of the griffin jQuery scripts can take advantage of the classes in this namespace.
    /// </para>
    /// <para>
    /// There are for instance a class which generates JSON from the model state which returns all errors (so that they can be nicely
    /// handled by the client). Another class uses the <c>ModelValidatorProvider</c> to generate all rules used by the jQuery.validation script.
    /// </para>
    /// <para>
    /// Note that this namespace is dependent on the JSON.Net serializer since the <c>DataContractJsonSerializer</c> can't handle <code>IEnumerable{T}</code>
    /// and the <c>JavascriptSerializer</c> can't handle custom property names (and I don't want to use camelCase property names). You therefore
    /// have to use the extension methods in <see cref="ControllerExtensions"/> to get proper serialization. Simly type <code>this.JsonResponse(</code> in the
    /// controller.
    /// </para>
    /// </remarks>
    /// <seealso cref="JsonSerializer"/>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}