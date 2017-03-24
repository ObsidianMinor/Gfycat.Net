namespace Gfycat
{
    /// <summary>
    /// Exposes the GfycatClient to objects
    /// </summary>
    public abstract class Entity
    {
        protected internal GfycatClient Client { get; }

        public string Id { get; internal set; }

        internal Entity(GfycatClient client, string id)
        {
            Client = client;
            Id = id;
        }
    }
}
