
using Daenet.ApiKeyAuthenticator;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestRestApi
{

    public class RoleGetter : IRoleGetter
    {  
        public Task<ICollection<string>> GetRoles(string userIdentifier)
        {
            return Task.FromResult<ICollection<string>> (new List<string> { "writer", "reader"});
        }
    }
}
