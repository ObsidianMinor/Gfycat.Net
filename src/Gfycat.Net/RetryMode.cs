namespace Gfycat
{
    /// <summary>
    /// Specifies retry modes for requests in the event they fail for any particular reason
    /// </summary>
    [System.Flags]
    public enum RetryMode
    {
        /// <summary>
        /// All responses don't retry
        /// </summary>
        AlwaysFail = 1,
        /// <summary>
        /// In the event of a task timeout, the request is attempted again
        /// </summary>
        RetryTimeouts = 2,
        /// <summary>
        /// In the event of 502 (Bad Gateway), the request is attempted again
        /// </summary>
        Retry502 = 4,
        /// <summary>
        /// In the event of 401, this will refresh the access token if possible and retry the request. If that request fails, an exception is thrown
        /// </summary>
        RetryFirst401 = 8,
        /// <summary>
        /// Retries the request if it times out, if it has a bad gateway, and on the first 401
        /// </summary>
        AlwaysRetry = RetryTimeouts | Retry502 | RetryFirst401
    }
}
