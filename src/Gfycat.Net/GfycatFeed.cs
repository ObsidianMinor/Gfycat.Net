﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gfycat
{
    public class GfycatFeed
    {
        [JsonProperty("gfycats")]
        public IEnumerable<Gfy> Gfycats { get; private set; }
        [JsonProperty("cursor")]
        public string Cursor { get; private set; }
    }
}
