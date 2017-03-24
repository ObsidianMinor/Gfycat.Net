using Newtonsoft.Json;

namespace Gfycat
{
    public class GfycatOperation
    {
        public GfycatOperation(Operation op, OperationPath path, object value)
        {
            Operation = op;
            Path = path;
            Value = value;
        }

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

        [JsonIgnore]
        public OperationPath Path { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }

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
