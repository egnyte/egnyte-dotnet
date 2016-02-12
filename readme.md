# Egnyte.NET

A .NET SDK for integrating with Egnyte API.

## Setup

To get started with Egnyte.NET, we recommend you add it to your project using NuGet.

To install `Egnyte.Api`, run the following command in the Package Manager Console:

```PM> Install-Package Egnyte.Api```

## Supported frameworks

* .Net Framework 4.5
* Windows Phone 8.1
* Xamarin

## Creating an application

You need to create an account to have a domain and generate a key for your application:

- Go to https://developers.egnyte.com

## How to use

### Obtaining an access token

In Egnyte.Api all 3 authorization flows are implemented. However, Authorization Flow is the most common. To create authorize uri, use OAuthHelper class:
```csharp
var authorizeUrl = OAuthHelper.GetAuthorizeUri(
    OAuthAuthorizationFlow.Code,
    Domain,
    PrivateKey,
    "https://mywebsite.com/redirectEgnyteResponse");
```

After getting the response, token can be obtained like this:

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
var listing = await client.Files.ListFileOrFolder("Shared/Documents/");
```

For more details, read methods descriptions or documentation.

## Documentation

You can documentation of Egnyte SDK here: https://developers.egnyte.com/docs.
