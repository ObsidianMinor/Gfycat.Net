namespace Gfycat
{
    [System.Flags]
    public enum RetryMode
    {
        AlwaysFail = 1,
        RetryTimeouts = 2,
        Retry502 = 4,
        /// <summary>
        /// In the event of 401, this will refresh the access token if possible and retry the request. If that request fails, an exception is thrown
        /// </summary>
        RetryFirst401 = 8,
        AlwaysRetry = RetryTimeouts | Retry502 | RetryFirst401
    }
}
