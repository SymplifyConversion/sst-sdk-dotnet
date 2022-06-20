# Symplify Server-Side Testing SDK for C#

This is the C# implementation of the Symplify Server-Side Testing SDK.

It is a cross platform .NET Core library, to enable integration regardless of
e.g. web frameworks used for building applications.

## Changes

See [CHANGELOG.md](CHANGELOG.md).

## Requirements

- [.NET] 6

[.NET]: https://dotnet.microsoft.com/en-us/download

## Installing

Using the dotnet CLI:

```shell
dotnet add package Symplify.Conversion.SDK --prerelease
```

https://www.nuget.org/packages/Symplify.Conversion.SDK has installation
instructions for the different .NET development environments.

## Usage

On your application startup:

1. Create an SDK instance (it's meant to be used as a singleton, you can wrap it in a service).
2. Call LoadConfig to start its config updater loop.

```csharp
// `yourWebsiteID` comes from your config.
// `httpClient` is an HttpClient, probably injected.
// `LoadConfig` is async and an await ensures the initial configuration loading is complete but as long as you call it it will eventually be loaded, so awaiting is not strictly necessary.
var sstSDK = new SymplifyClient(yourWebsiteID, httpClient);
await sstSDK.LoadConfig();
```

On each request where you want to use variations:

1. Call findVariation, providing a `CookieJar` (see below)

```csharp
// `cookieJar` is an ICookieJar you need to create, see the CookieJar section
sstSDK.FindVariation("my ab-test", cookieJar)
```

See the project `Symplify.Conversion.SDK.DemoApp` for more elaborate example
code. Running instructions for it are in [CONTRIBUTING.md](CONTRIBUTING.md).

### CookieJar

In order to assign variations stably in face of configuration changes, we need
to persist the allocation for each user somehow. The SST SDK uses cookies for
this (which also helps integration with the frontend js-sdk).

To be compatible with any web framework, the SDK uses an interface `ICookieJar`
which you implement to provide reading and writing of cookies. Here is an
example using `IHttpContextAccessor` which you can use in eveyr request you
handle:

```csharp
using Microsoft.AspNetCore.Http;
using Symplify.Conversion.SDK.Cookies;

class HttpContextCookieJar : ICookieJar
{
    private readonly string domain;
    private readonly IHttpContextAccessor accessor;

    public HttpContextCookieJar(string domain, IHttpContextAccessor accessor)
    {
        this.domain = domain;
        this.accessor = accessor;
    }

    public string GetCookie(string name)
    {
        return accessor.HttpContext.Request.Cookies[name];
    }

    public void SetCookie(string name, string value, uint expireInDays)
    {
        CookieOptions opts = new();
        opts.Domain = domain;
        opts.Expires = DateTimeOffset.Now.AddDays(expireInDays);
        accessor.HttpContext.Response.Cookies.Append(name, value, opts);
    }
}
```

If you run test on a site with multiple subdomains, you will need to use a
common "parent" domain for the cookies, such as `".example.com"` for e.g.
`"b2b.example.com"` and `"store.example.com"`.

## SDK Development

See [CONTRIBUTING.md](CONTRIBUTING.md) or [RELEASING.md](RELEASING.md).
