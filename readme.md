# Egnyte.NET

A .NET SDK for integration with Egnyte API.

## Setup

To get started working with Egnyte .NET SDK, we recommend you to add it to your project using NuGet.

To install `Egnyte.Api`, run the following command in the Package Manager Console:

```PM> Install-Package Egnyte.Api```

## Supported frameworks
Version number 2 of the package was written in .Net Standard, to meet the latest technological trends and ease project development. .Net Standard 2.0 supports quite a lot of frameworks, details can be found here: https://docs.microsoft.com/pl-pl/dotnet/standard/net-standard

Currently there are two versions available. However, version 0.1.* will not me maintained.
* Version 2
  * .Net Standard 2.0
  * .Net Framework 4.6.1
* Version 0.1.*
  * .Net Framework 4.5
  * Windows Phone 8.1
  * Xamarin


## Creating an application

You need to create an account to have a domain and generate a key for your application:

- Go to https://developers.egnyte.com

## How to use

### Obtaining an access token

In Egnyte.Api all 3 authorization flows are implemented. However, Authorization Code Flow is the most common. To create authorize uri, use OAuthHelper class:
```csharp
var authorizeUrl = OAuthHelper.GetAuthorizeUri(
    OAuthAuthorizationFlow.Code,
    Domain,
    PrivateKey,
    "https://mywebsite.com/redirectEgnyteResponse");
```

You will need to redirect user to that url, so he can enter he's credentianls. After getting the response, token can be obtained like this:

```csharp
var token = await EgnyteClientHelper.GetTokenFromCode(
	Domain,
    PrivateKey,
    Secret,
    "https://mywebsite.com/redirectEgnyteResponse",
	code);
```

### API operations using token

To perform Egnyte API operations, please create EgnyteClient:

```csharp
var client = new EgnyteClient(Token, Domain);

// lists files and folders in Documents directory
var listing = await client.Files.ListFileOrFolder("Shared/Documents");
```

For more details, read methods descriptions or documentation.

## Documentation

You can browse full documentation of Egnyte SDK here: https://developers.egnyte.com/docs.

## Known issues

When using Egnyte.Api package with .Net Framework older then 4.6, you might run into a securitu protocol issues. If you run into this error message:
> An error occurred while making the HTTP request to https://<API endpoint>. This could be due to the fact that the server certificate is not configured properly with HTTP.SYS in the HTTPS case. This could also be caused by a mismatch of the security binding between the client and the server.

you might need to add this line to your application:
`ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12`

and if that does not help, you might need to update your .Net Framework to 4.6 or higher. Please take a look at [this article](https://blogs.perficient.com/2016/04/28/tsl-1-2-and-net-support/) for more details.

## License

MIT License

https://opensource.org/licenses/MIT
