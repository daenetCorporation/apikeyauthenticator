using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace TestRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyApiController : ControllerBase
    {
        private readonly ILogger<MyApiController> _logger;

        public MyApiController(ILogger<MyApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        [AllowAnonymous]
        public string GetAnonymous()
        {
            var res = ReadIdentities();

            return res;
        }

        [HttpGet()]
        [Route("authorized")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public string GetAuthorized()
        {
            // Gets the name of authenticated user.
            var user = this?.User?.Identity?.Name;

            var res = ReadIdentities();

            return res;
        }

        /// <summary>
        /// This method demonstrates impersonation. The client must send the header "ImpersonateTo"
        /// (you can change this inside <see cref="nameof(ClaimsBuilder)"/>), which contains the name 
        /// of the principal (user), for which the Claims Principal will be created.
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Route("impersonated")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public string GetImpersonated()
        {
            string? impersonatedTo = this.User?.Identity?.Name;
            string? impersonatedFrom = this?.User?.Claims.FirstOrDefault(c => c.Type == "ImpersonatedFrom")?.Value;

            var res = ReadIdentities();

            return res ;
        }


        [HttpGet()]
        [Route("anonymous")]
        [AllowAnonymous()]
        [Authorize(Roles = "admin, contributor")]
        public string Ping()
        {
            return DateTime.UtcNow.ToString(); 
        }

        protected string ReadIdentities()
        {
            StringBuilder sb = new StringBuilder();
         
            foreach (var identity in User.Identities)
            {
                sb.AppendLine($"--- Identity: {identity.Name} ---");

                sb.AppendLine($"Authentication Type: {identity.AuthenticationType}");

                foreach (var claim in identity.Claims)
                {
                    sb.AppendLine($"Claim: {claim.Type}, Value: {claim.Value}");
                }
            }

            Debug.WriteLine(sb.ToString());

            return sb.ToString();
        }
    }
}