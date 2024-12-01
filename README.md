
# API Key Authentication with `Daenet.ApiKeyAuthenticator`

## Overview

API Key authentication is a straightforward method for authenticating clients to access an API. It involves sending an API key, a unique identifier issued by the server, alongside the API request. The server validates the API key to verify the client's authorization and then processes the request accordingly.

While API Key authentication is commonly used for public APIs and provides basic security, it is not considered highly secure for sensitive operations or data due to its simplicity.
This repository provides a NuGet package that integrates the API Key authentication with a custom authorization option in .NET applications.

---

## Installation

To integrate the `Daenet.ApiKeyAuthenticator` library in your .NET project, use the following NuGet command:

```bash
dotnet add package Daenet.ApiKeyAuthenticator --version 1.0.2
```

## Using API Key Authenticator

To enable API Key authentication for an API controller in ASP.NET follow these steps:

### 1. Activating Authentication for Specific Operations

To authenticate a specific controller operation, decorate the operation with the `Authorize` attribute using the `ApiKey` authentication scheme.

#### Example:

```csharp
[HttpGet]
[Route("authorized")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public IActionResult GetAuthorized()
{
    // Retrieves the name of the authenticated user.
    var user = this?.User?.Identity?.Name;

    return Ok(user);
}
```

### 2. Activating Authentication at the Controller Level

To enforce API Key authentication for all methods within a controller, apply the `Authorize` attribute at the controller level.

#### Example:

```csharp
[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public class MyApiController : ControllerBase
{
    // All methods in this controller require API Key authentication.
}
```

### 3. Allowing Anonymous Access to Specific Methods

If a controller is secured with `Authorize` but you want to allow anonymous access to specific operations, use the `[AllowAnonymous]` attribute for those methods.

#### Example:

```csharp
[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "ApiKey")]
public class MyApiController : ControllerBase
{
    [HttpGet]
    [Route("public")]
    [AllowAnonymous]
    public IActionResult MethodAllowedForAnonymousUser()
    {
        return Ok("This endpoint is publicly accessible.");
    }
}
```

### 4. Important Note When Setting [Anonymous] at the controller level

Do **not** decorate an entire controller with `[AllowAnonymous]` if at least one method requires API Key authentication. This would make all endpoints within the controller accessible without authentication, even if the method is decorated with the `[Authorize]` attribute.

### 5. Invoking Operations with Anonymous User
When some operation on the controller does not need to be authenticated, the *ApiKeyAuthenticateor* simply should not be activated.

### 6. # Using `CustomClaimsBuilder` 
The `ICustomClaimsBuilder` interface defines a robust contract for components that generate additional claims to be appended to the `ClaimsPrincipal` in the context of a given request. This interface is instrumental in extending and customizing the claims-based authentication process by dynamically creating claims based on the request and user-specific information.

This approach is particularly valuable when the identity server does not supply all the claims required by the application. By leveraging a custom claims builder, developers can modify and enhance the authenticated principal's claims dynamically, enabling unparalleled flexibility in implementing fine-grained authorization mechanisms within the application.

Benefits of this approach:

- **Dynamic Claim Extension**: Generate claims dynamically based on application-specific requirements.
- **Decoupled Logic**: Keep the identity server minimal while enriching claims on demand at the application level.
- **Flexible Authorization**: Support advanced authorization scenarios by manipulating claims in the principal.
- **Scalable Security**: Easily adapt claims-based logic without modifying the underlying authentication system.

### 7. Impersonating User with `ApiKeyAuthenticator`

In certain scenarios, an API needs to act on behalf of another user, a process known as **impersonation**. Impersonation allows an API to execute actions in the context of a user specified by the client, even when the API is invoked using service credentials or the context of a different user.

To support impersonation, the client must provide the **`Impersonating`** header in the API request. The `ApiKeyAuthenticator` middleware will authenticate the invoking user and inject an additional identity into the list of identities associated with the principal. This additional identity represents the user who will be impersonated.

#### Why Impersonation is Needed

Impersonation is useful in scenarios such as:
- **Delegated Access**: A service or system acts on behalf of another user to perform specific operations.
- **Admin Privileges**: An administrator performs actions as another user for troubleshooting or support purposes.
- **Service Integration**: Systems with multiple roles or contexts require operations to be executed under different user identities.

#### Implementation Details

To enable impersonation, the client must:
1. Authenticate using service principal or as a specific user.
2. Include the `ImpersonatingUser` header in the API request, specifying the user to impersonate.

##### Example Request:
GET /api/resource Authorization: ApiKey <service-api-key> Impersonating: username-to-impersonate


When the API receives a request with the `ImpersonatingUser` (this name can be changed in the configuration) header:
1. **Authentication**: The `ApiKeyAuthenticator` validates the invoking user's API key.
2. **Identity Injection**: After successful authentication, the middleware creates the identity of the caller (service principal or a specific user) and additionally injects an identity into the `ClaimsPrincipal` object. This identity represents the user being impersonated.
3. **Identity Management**: The `ClaimsPrincipal` now contains:
   - The invoking user's identity.
   - The impersonated user's identity.

     
## Summary

The `Daenet.ApiKeyAuthenticator` library enables seamless integration of API Key-based authentication in .NET applications. Use this guide to implement, secure, and customize API Key authorization for your API endpoints.

For more details, refer to the official documentation or example implementation provided in this repository.

