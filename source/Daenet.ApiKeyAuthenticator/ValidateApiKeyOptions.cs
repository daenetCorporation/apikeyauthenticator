using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.ApiKeyAuthenticator
{
    public class ValidateApiKeyOptions : AuthenticationSchemeOptions
    {
        public override void Validate()
        {
           
        }

        public override void Validate(string scheme)
        {
            
        }
    }
}
