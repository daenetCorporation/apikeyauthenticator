using Daenet.ApiKeyAuthenticator;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace TestRestApi
{
    public class ClaimsBuilder : ICustomClaimsBuilder
    {
        public Task<IList<Claim>> GetClaims(string userIdentifier, HttpRequest request)
        {
            var customClaims = new List<Claim>();

            StringValues impersonatedUser;

            if (request.Headers.TryGetValue("ImpersonateTo", out impersonatedUser))
            {
                customClaims.Add(new Claim(ClaimTypes.Name, impersonatedUser.ToString()));
                customClaims.Add(new Claim("ImpersonatedFrom", userIdentifier));
            }
            else
            {
                customClaims.Add(new Claim(ClaimTypes.Name, "haso mujo"));
            }

            customClaims.Add(new Claim(ClaimTypes.NameIdentifier, "todo"));
           
            // Other claims here.

            return Task.FromResult<IList<Claim>>(customClaims);
        }

        public Task<IList<Claim>> GetClaims(HttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
