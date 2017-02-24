using Newtonsoft.Json;

namespace Gfycat
{
    public class GfycatOperation
    {
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
                    case CurrentUserOperationPath.Name:
                        return "/name";
                    case CurrentUserOperationPath.Email:
                        return "/email";
                    case CurrentUserOperationPath.Password:
                        return "/password";
                    case CurrentUserOperationPath.ProfileUrl:
                        return "/profile_url";
                    case CurrentUserOperationPath.Description:
                        return "/description";
                    case CurrentUserOperationPath.UploadNotices:
                        return "/upload_notices";
                    case CurrentUserOperationPath.DomainWhitelist:
                        return "/domain_whitelist";
                    case CurrentUserOperationPath.GeoWhitelist:
                        return "/geo_whitelist";
                    case CurrentUserOperationPath.IframeImageVisible:
                        return "/iframe_image_visible";
                    default:
                        return string.Empty;
                }
            }
        }
        public CurrentUserOperationPath Path { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }

    public enum Operation
    {
        Add,
        Remove,
        Replace
    }

    public enum CurrentUserOperationPath
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
