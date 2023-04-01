# apikeyauthenticator
API key authentication is a simple method of authentication that involves sending an API key along with an API request to the server. The API key is a unique identifier that is issued by the server to the client, and it is used to authenticate the client when making API requests. The server checks the API key to ensure that the client has permission to access the API, and then processes the API request accordingly. API key authentication is commonly used for public APIs and provides a basic level of security, but it is not considered a highly secure method of authentication.

This repository provides a library, an example and a nuget package that enambles easy integration of the ApiKey authentication and authorization in .NET applications.

Distribution via nuget:
~~~
dotnet add package Daenet.ApiKeyAuthenticator --version 1.0.2
~~~
