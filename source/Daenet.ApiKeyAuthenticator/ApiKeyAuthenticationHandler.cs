using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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
    {     
        Task<ICollection<string>> GetRoles(string userIdentifier);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ICustomClaimsBuilder
    {
        Task<IList<Claim>> GetClaims(HttpRequest request);
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
        private ApiKeyConfig _cfg;

        /// <summary>
        /// Delegate for getting the roles.
        /// </summary>
        private readonly IRoleGetter _roleGetter;

        /// <summary>
        /// The purpose of the ICustomClaimsBuilder interface is to provide a mechanism for retrieving the list of custom claims associated with a given request. 
        /// </summary>
        private readonly ICustomClaimsBuilder _claimBuilder;

        /// <summary>
        /// Creates the instance of the handler.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        /// <param name="cfg"></param>
        /// <param name="roleGetter">The action used to return the list of roles of the principal.</param>
        /// <param name="principalGetter">The purpose of the ICustomClaimsBuilder interface is to provide a mechanism for retrieving the list of custom claims associated with a given request. 
        /// By allowing the caller to optionally pass in their own implementation of this interface, 
        /// the constructor provides a flexible way to customize the behavior of the object without requiring any changes 
        /// to the code itself.</param>
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ValidateApiKeyOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ApiKeyConfig cfg,
            IRoleGetter roleGetter = null, ICustomClaimsBuilder principalGetter = null)
            : base(options, logger, encoder)
        {
            this._roleGetter = roleGetter;
            this._claimBuilder =  principalGetter;
            this._cfg = cfg;
        }

        /// <summary>
        /// It checks for the ApiKey header and validates if the provided key is the registered in the application configuration.
        /// </summary>
        /// <returns></returns>
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            StringValues keyValue;

            //
            // This is the case when the Authorization is required and header does not contain the ApiKey.
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out keyValue))
            {
                return AuthenticateResult.Fail($"Header '{ApiKeyHeaderName}' not found.");
            }

            foreach (var item in this._cfg.Keys)
            {
                if (item.KeyValue == keyValue)
                {
                    List<Claim> claims = new List<Claim>();

                    if (_claimBuilder != null)
                    {
                        var customClaims = await _claimBuilder.GetClaims(Request);
                        claims.AddRange(customClaims);
                    }

                    var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                 
                    // If custom claims do not provide the principal, then the principal name specified in the configuraiton is used.
                    if (nameClaim is null)
                        claims.Add(nameClaim = new Claim(ClaimTypes.Name, item.PrincipalName));

                    string principalName = nameClaim.Value;

                    claims.Add(new Claim(ClaimTypes.AuthenticationMethod, "ApiKey"));

                    ICollection<string> roles;

                    if (_roleGetter != null)
                    {
                        roles = await _roleGetter.GetRoles(item.PrincipalName);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "ApiKey"));

                    var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

                    //Request.HttpContext.User.AddIdentity(appIdentity);
                    return AuthenticateResult.Success(ticket);
                }
            }

            return AuthenticateResult.Fail("Invalid key specified.");
        }
    }
}
