# Symplify Server-Side Testing SDK for C#

This is the C# implementation of the Symplify Server-Side Testing SDK.

It is a cross platform .NET Core library, to enable integration regardless of
e.g. web frameworks used for building applications.

## Requirements

- [.NET] 6

[.NET]: https://dotnet.microsoft.com/en-us/download

## Installing

NuGet package Coming soon...

## Usage

On your application startup:

1. Create an SDK instance.
2. Call LoadConfig to start its config updater loop.

On each request where you want to use variations:

1. Call findVariation, providing a `CookieJar` (see below)

See example usage in the project `SymplifySDK.DemoApp`

### CookieJar

In order to assign variations stably in face of configuration changes, we need
to persist the allocation for each user somehow. The SST SDK uses cookies for
this (which also helps integration with the frontend js-sdk).

To be compatible with any web framework, the SDK uses an interface `ICookieJar`
which you implement to provide reading and writing of cookies.

## SDK Development

Requirements:

- [Caddy](https://caddyserver.com)

### Local Unit Testing

The project `SymplifySDK.Tests` contains all the test files. Run `dotnet test`
to run the test locally.

### Fake CDN

To not rely on production servers for testing, you can use the fake CDN in the
[caddy](caddy) directory for serving config files.

```shell
$ cd caddy/fakeCDN
$ caddy
```

For its hostname (fake-cdn.localhost.test) to be usable, you need to add it to
your [hosts file], for 127.0.0.1.

### Run the example application

The project `SymplifySDK.DemoApp` is a ASP.NET Core application with Razor
pages. It just shows an HTML page listing all the configured projects.
To Run the application: `dotnet run --project SymplifySDK.DemoApp`.

You can now browse the site at http://127.0.0.1:61265. There is a Caddyfile in
[caddy](caddy) you can use if you want to browse
https://symplify-demoapp.localhost.test instead (i.e. using TLS).

This example app uses a service created for providing the SDK functionality (see
`services/SymplifyService`). See `Startup.cs` file in the `ConfigureServices` function.

In the index.cshtml we find this snippet
`@Model.client.FindVariation(projectName, Model)` which is the main feature of
the SDK. Model is passed in because it's how the example implements `ICookieJar`
(see [Usage](#Usage)) since it has access to the request and response cookies.

[hosts file]: https://en.wikipedia.org/wiki/Hosts_(file)
