using System.Threading.Tasks;

namespace Gfycat.OAuth2
{
    public interface IAuthenticator
    {
        string AccessToken { get; }
        string RefreshToken { get; }
        string ResourceOwner { get; }
        string Scope { get; }

        Task RefreshTokenAsync(string providedToken = null);

        Task AuthenticateAsync();
        Task AuthenticateAsync(string username, string password);
        Task AuthenticateAsync(TokenType type, string token);
        Task AuthenticateAsync(TokenType type, string token, string verifierOrSecret);
    }
}
