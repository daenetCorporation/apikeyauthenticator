using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Daenet.ApiKeyAuthenticator
{
    /// <summary>
    /// Provides the interface for a component that will deliver the list of roles of the principal.
    /// </summary>
    public interface IRoleGetter
    {        Task<ICollection<string>> GetRoles(string userIdentifier);
    }

    /// <summary>
    /// Provides a support for ApiKey authentication.
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ValidateApiKeyOptions>
    {
        private const string ApiKeyHeaderName = "ApiKey";

        /// <summary>
        /// The handler configuration.
        /// </summary>
        private ApiKeyConfig cfg;

        private IRoleGetter roleGetter;

        /// <summary>
        /// Creates the instance of the handler.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="cfg"></param>
        /// <param name="roleGetter">The action used to return the list of roles of the principal.</param>
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ValidateApiKeyOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ApiKeyConfig cfg,
            IRoleGetter roleGetter = null)
            : base(options, logger, encoder, clock)
        {
            this.roleGetter = roleGetter;
            this.cfg = cfg;
        }


        /// <summary>
        /// It checks for the ApiKey header and validates if the provided key is the registered in the application configuration.
        /// </summary>
        /// <returns></returns>
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            StringValues keyValue;

            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out keyValue))
            {
                return AuthenticateResult.Fail($"Header '{ApiKeyHeaderName}' not found.");
            }

            foreach (var item in this.cfg.Keys)
            {
                if (item.KeyValue == keyValue)
                {
                    Claim claimName = new Claim(ClaimTypes.Name, item.PrincipalName);
                    var apiKeyClaim = new Claim("apikey", keyValue);
                    var subject = new Claim(ClaimTypes.NameIdentifier, ApiKeyHeaderName);

                    ICollection<string> roles;

                    var claims = new List<Claim> { apiKeyClaim, subject, claimName };

                    if (roleGetter != null)
                    {
                        roles = await roleGetter.GetRoles(item.PrincipalName);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, item.PrincipalName));

                    var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

                    //Request.HttpContext.User.AddIdentity(appIdentity);
                    return AuthenticateResult.Success(ticket);
                }
            }

            return AuthenticateResult.Fail("Invalid key specified.");
        }
    }
}
