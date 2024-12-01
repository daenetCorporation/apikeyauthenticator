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

            if (userIdentifier == "Key1-BasicSKU")
            {
                customClaims.Add(new Claim(ClaimTypes.Country, "Germany"));
                customClaims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Basic Subscription Service"));
            }
            else if (userIdentifier == "Key2-PremiumSKU")
            {
                customClaims.Add(new Claim(ClaimTypes.Country, "Germany"));
                customClaims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Premium Subscription Service"));
            }
            else
            {
                if (request.Headers.TryGetValue("ImpersonatingUser", out impersonatedUser))
                {
                    customClaims.Add(new Claim(ClaimTypes.Country, "Germany"));
                    customClaims.Add(new Claim(ClaimTypes.AuthorizationDecision, "ImpersonatingUser"));
                    customClaims.Add(new Claim("ImpersonatedFrom", userIdentifier));
                }
            }           

            return Task.FromResult<IList<Claim>>(customClaims);
        }

        public Task<IList<Claim>> GetClaims(HttpRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
