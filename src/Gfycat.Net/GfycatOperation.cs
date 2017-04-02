using Newtonsoft.Json;

namespace Gfycat
{
    /// <summary>
    /// Specifies an operation to modify a part of the current user's setting. See "http://developers.gfycat.com/api/#updating-user-39-s-details" for details on valid operation combinations
    /// </summary>
    /// <remarks>See http://developers.gfycat.com/api/#updating-user-39-s-details for details on valid operations</remarks>
    public class GfycatOperation
    {
        /// <summary>
        /// Constructs a new operation with the given operation, path, and value
        /// </summary>
        /// <param name="op"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public GfycatOperation(Operation op, OperationPath path, object value)
        {
            Operation = op;
            Path = path;
            Value = value;
        }
        
        /// <summary>
        /// Sets the operation to perform on the specified path
        /// </summary>
        [JsonIgnore]
        public Operation Operation { get; set; } // I couldn't use the string enum converter with an array of constructor args because CLS compliance
        [JsonProperty("op")]
        private string _operation => Operation.ToString().ToLowerInvariant();

        [JsonProperty("path")]
        private string _path
        {
            get
            {
                switch(Path)
                {
                    case OperationPath.Name:
                        return "/name";
                    case OperationPath.Email:
                        return "/email";
                    case OperationPath.Password:
                        return "/password";
                    case OperationPath.ProfileUrl:
                        return "/profile_url";
                    case OperationPath.Description:
                        return "/description";
                    case OperationPath.UploadNotices:
                        return "/upload_notices";
                    case OperationPath.DomainWhitelist:
                        return "/domain_whitelist";
                    case OperationPath.GeoWhitelist:
                        return "/geo_whitelist";
                    case OperationPath.IframeImageVisible:
                        return "/iframe_image_visible";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// The item to edit in this operation
        /// </summary>
        [JsonIgnore]
        public OperationPath Path { get; set; }

        /// <summary>
        /// The new value for this operation
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }
    }

#pragma warning disable CS1591
    public enum Operation
    {
        Add,
        Remove,
        Replace
    }

    public enum OperationPath
    {
        Name, 
        Email,
        Password,
        ProfileUrl,
        Description,
        UploadNotices,
        DomainWhitelist,
        GeoWhitelist,
        IframeImageVisible
    }
}
