namespace Griffin.MvcContrib.Json
{
    /// <summary>
    /// A response sent back to the client
    /// </summary>
    /// <remarks>
    /// <para>
    /// It can be hard to handle JSON responses for actions which does something other than simple
    /// gets. For instance a create method might want to return a user if everything went OK or 
    /// an error message if something failed.
    /// </para>
    /// <para>By encapsulating all responses in a predefined structure we know what kind of
    /// response we get for each request and can therefore handle it accordingly.</para>
    /// </remarks>
    /// <seealso cref="JsonResponse"/>
    public interface IJsonResponseContent
    {
    }
}