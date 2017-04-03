namespace Gfycat
{
    /// <summary>
    /// Reprsents an gfy upload's current task
    /// </summary>
    public enum UploadTask
    {
        /// <summary>
        /// Represents an invalid upload task
        /// </summary>
        Invalid, // if for some reason json.net just can't figure it out or they didn't give us anything 
        /// <summary>
        /// Represents an upload that doesn't exist
        /// </summary>
        NotFoundo, // I'm so serious, that's how it's spelled...
        /// <summary>
        /// Represents an upload that is encoding
        /// </summary>
        Encoding,
        /// <summary>
        /// Represents an upload which has encountered an error
        /// </summary>
        Error,
        /// <summary>
        /// Represents an upload which is complete
        /// </summary>
        Complete
    }
}
