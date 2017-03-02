﻿using Gfycat.Rest;
using System;
using System.Reflection;

namespace Gfycat
{
    public class GfycatClientConfig
    {
        public const int ApiVersion = 1;
        public static readonly string BaseUrl = $"https://api.gfycat.com/v{ApiVersion}/";
        public static Version Version => typeof(GfycatClient).GetTypeInfo().Assembly.GetName().Version;

        /// <summary>
        /// Overrides the rest client for this Gfycat client
        /// </summary>
        public IRestClient RestClient { get; set; }

        /// <summary>
        /// Sets the default retry mode for all requests, the default is <see cref="RetryMode.RetryFirst401"/>
        /// </summary>
        public RetryMode DefaultRetryMode { get; set; } = RetryMode.RetryFirst401;
        public int? DefaultTimeout { get; set; }

        public GfycatClientConfig()
        {
            RestClient = new DefaultRestClient(new Uri(BaseUrl));
        }
    }
}