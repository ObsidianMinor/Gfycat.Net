using System.Diagnostics;

namespace Gfycat
{
    /// <summary>
    /// Exposes the GfycatClient to objects
    /// </summary>
    public abstract class Entity
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal GfycatClient Client { get; }
        /// <summary>
        /// Gets the id of this entity
        /// </summary>
        public string Id { get; internal set; }

        internal Entity(GfycatClient client, string id)
        {
            Client = client;
            Id = id;
        }
    }
}
