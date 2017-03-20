using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat
{
    internal class CurrentUserSearchFeed : GfyFeed
    {
        public CurrentUserSearchFeed(GfycatClient client, string searchText, RequestOptions options, int count) : base(client)
        {
        }
    }
}
