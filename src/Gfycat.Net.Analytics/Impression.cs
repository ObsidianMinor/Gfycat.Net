namespace Gfycat.Analytics
{
    /// <summary>
    /// Represents an gfy impression
    /// </summary>
    public class Impression
    {
        /// <summary>
        /// Sets the gfy this impression is for
        /// </summary>
        public Gfy Gfycat { get; set; }
        /// <summary>
        /// Sets the context of this impression
        /// </summary>
        public ImpressionContext Context { get; set; }
        /// <summary>
        /// Sets the keyword of this impression
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Sets the flow of this impression
        /// </summary>
        public ImpressionFlow Flow { get; set; }
        /// <summary>
        /// Sets the view tag of this impression
        /// </summary>
        public string ViewTag { get; set; }
    }
}
