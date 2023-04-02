using Daenet.ApiKeyAuthenticator;
using System.Security.Claims;

namespace TestRestApi
{
    public class ClaimsBuilder : ICustomClaimsBuilder
    {
        public Task<IList<Claim>> GetClaims(HttpRequest request)
        {
            var customClaims = new List<Claim>();
            customClaims.Add(new Claim(ClaimTypes.Name, "haso mujo"));
            customClaims.Add(new Claim(ClaimTypes.NameIdentifier, "haso@mujo.org"));
            return Task.FromResult<IList<Claim>>(customClaims);
        }
    }
}
