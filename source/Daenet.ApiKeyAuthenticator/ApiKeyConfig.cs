using System.Collections.Generic;

namespace Daenet.ApiKeyAuthenticator
{
    /// <summary>
    /// Defines the configuration for ApiKey authentication.
    /// </summary>
    public class ApiKeyConfig
    {
        /// <summary>
        /// The list of allowed keys and assotiated principal names
        /// </summary>
        public List<KeyDefinition> Keys { get; set; }
    }

    /// <summary>
    /// Definition of the single key.
    /// </summary>
    public class KeyDefinition
    {
        /// <summary>
        /// The name of the principal that will hold the claim.
        /// </summary>
        public string PrincipalName { get; set; }

        /// <summary>
        /// The value of the key.
        /// </summary>
        public string KeyValue { get; set; }

    }
}
