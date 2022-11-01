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
dotnet add package Symplify.Conversion.SDK
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

### Custom audience

It's possible to limit for which requests/visitors a certain test project
should apply by using "audience" rules. See [Audiences.md](https://github.com/SymplifyConversion/sst-documentation/blob/main/docs/Audicences.md)
for details.

The audience is evaluated when your server calls `findVariation`, and if the
rules you have setup in the audience references "custom attributes" your
server must provide the values of these attributes for each request.

For example, you might want a test project to only apply for visitors from a
certain country. The audience can be configured in your project, using a
custom attribute "country", and then your server provides it when finding the
variation on each request:

```csharp
    // fictional helper function to get discounts for each request we serve
   public double[] getDiscounts(Client sdk, ICookieJar cookieJar)
    {
        // This code assumes you have a `LookupGeoIP` helper function in your project.
        JArray customAttributes = JArray.Parse($"[{{'country': '{LookupGeoIp(usersIPAddress).GetCountry()}'}}]");
        
        // Custom attributes are passed as an JArray, in this case we set 'country'
        // and assume the audience is configured with the "string-attribute" rule to look for specific countries.
        string variation = sdk.FindVariation("Discounts, May 2022", cookieJar, customAttributes);

        switch (variation)
        {
            case "huge":
                return new double[1] { 0.25 };
            case "small":
                return new double[1] { 0.1 };
        }

        // `findVariation` returns empty array if the project audience does not match for
        // a given request. We handle that by a fallthrough return here.
        return new double[1];
    }
```

## SDK Development

See [CONTRIBUTING.md](CONTRIBUTING.md) or [RELEASING.md](RELEASING.md).
