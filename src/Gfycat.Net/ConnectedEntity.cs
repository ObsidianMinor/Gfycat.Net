namespace Gfycat
{
    /// <summary>
    /// Exposes the current web client to objects
    /// </summary>
    public abstract class ConnectedEntity
    {
        internal ExtendedHttpClient Web { get; set; }
    }
}
