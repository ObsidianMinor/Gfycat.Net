using System;
using System.Collections.Generic;

namespace Gfycat
{
    public class AppApiInfo : Entity
    {
        internal AppApiInfo(GfycatClient client, string id) : base(client, id)
        {
        }

        public string ContactName { get; private set; }
        public string WebUrl { get; private set; }
        public string Company { get; private set; }
        public DateTime CreationDate { get; private set; }
        public string AppName { get; private set; }
        public IReadOnlyCollection<string> RedirectUris { get; private set; }
        public AppType AppType { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
    }
}