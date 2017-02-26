namespace Gfycat
{
    /// <summary>
    /// Exposes the HTTP client to objects
    /// </summary>
    public abstract class ConnectedEntity
    {
        internal InternalClient Web { get; set; }
    }
}
