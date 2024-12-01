# apikeyauthenticator
API key authentication is a simple method of authentication that involves sending an API key along with an API request to the server. The API key is a unique identifier that is issued by the server to the client, and it is used to authenticate the client when making API requests. The server checks the API key to ensure that the client has permission to access the API, and then processes the API request accordingly. API key authentication is commonly used for public APIs and provides a basic level of security, but it is not considered a highly secure method of authentication.

This repository provides a library, an example and a nuget package that enables easy integration of the ApiKey authentication and authorization in .NET applications.

Distribution via nuget:
~~~
dotnet add package Daenet.ApiKeyAuthenticator --version 1.0.2
~~~


# Using ApiKey Authenticator
When an operation on the controller needs to be authenticated using ApiKey, the ApiKEyAuthenticator must first be activated.
The easiest way to activate the ApiKey authentication is to decorate the operation with the attribute **Authorize:

~~~csharp
[Authorize(AuthenticationSchemes = "ApiKey")]
~~~

The following method shows how to activate authentication and read the authenticated user.

~~~csharp
[HttpGet()]
[Route("authorized")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public IEnumerable<WeatherForecast> GetAuthorized()
{
    // Gets the name of the authenticated user.
    var user = this?.User?.Identity?.Name;

    . . . 
}
~~~

If you want to activate authentication for all methods inside a controller, you should provide the same attribute at the Controller level:

~~~csharp
 [ApiController]
 [Route("[controller]")]
 [Authorize(AuthenticationSchemes = "ApiKey")]
 public class MyApiController : ControllerBase
 {
    . . . 
 }
 ~~~

# Invoking Operations with Anonymous User
When some operation on the controller does not need to be authenticated, the *ApiKeyAuthenticateor* simply should not be activated.

# Invoking Operations with Impersonated User

