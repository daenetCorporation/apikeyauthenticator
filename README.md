
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

### Important Note:

Do **not** decorate an entire controller with `[AllowAnonymous]` if at least one method requires API Key authentication. This would make all endpoints within the controller accessible without authentication, even if the method is decorated with the [Authorization] attribute.

# Invoking Operations with Anonymous User
When some operation on the controller does not need to be authenticated, the *ApiKeyAuthenticateor* simply should not be activated.

# Invoking Operations with Impersonated User

## Summary

The `Daenet.ApiKeyAuthenticator` library enables seamless integration of API Key-based authentication in .NET applications. Use this guide to implement, secure, and customize API Key authorization for your API endpoints.

For more details, refer to the official documentation or example implementation provided in this repository.

