using System;
using System.Collections.Generic;
using System.Text;

namespace Gfycat
{
    public abstract class ConnectedEntity
    {
        internal ExtendedHttpClient Web { get; }

        internal ConnectedEntity(ExtendedHttpClient client)
        {
            Web = client;
        }
    }
}
