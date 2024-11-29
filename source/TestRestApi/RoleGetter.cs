
using Daenet.ApiKeyAuthenticator;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestRestApi
{

    public class RoleGetter : IRoleGetter
    {
        public Task<ICollection<string>> GetRoles(string userIdentifier, HttpRequest request)
        {
            return Task.FromResult<ICollection<string>> (new List<string> { "writer", "reader"});
        }

        public Task<ICollection<string>> GetRoles(string userIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
