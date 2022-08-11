## Setup

Requirements:

- [.NET](https://dotnet.microsoft.com/en-us/download)
- (for .NET Framework 4.7 support outside Windows) [Mono](https://www.mono-project.com/download/stable/)
- [Caddy](https://caddyserver.com)

1. Clone this repository
2. Run the test suite to verify things are working

```shell
git clone git@github.com:SymplifyConversion/sst-sdk-dotnet.git
cd sst-sdk-dotnet
dotnet test
```

## Unit Testing

The project `Symplify.Conversion.SDK.Tests` contains all the test code. Run
`dotnet test` to run the tests for all target frameworks.

If your `dotnet` CLI does not handle all target frameworks (e.g. you are on macOS), you will have to test them separately:

```shell
./ci/test_net6.0.sh
./ci/test_net47.sh
```

## Testing with a local site

See [the example](Symplify.Conversion.SDK.DemoApp/).

### Fake CDN

To not rely on production servers for testing, you can use the fake CDN in the
[caddy](caddy) directory for serving config files.

```shell
$ cd caddy/fakeCDN
$ caddy run
```

For its hostname (fake-cdn.localhost.test) to be usable, you need to add it to
your [hosts file], for 127.0.0.1.

### Run the example application

The project `Symplify.Conversion.SDK.DemoApp` is a ASP.NET Core application with Razor
pages. It just shows an HTML page listing all the configured projects.
To Run the application: `dotnet run --project Symplify.Conversion.SDK.DemoApp`.

You can now browse the site at http://127.0.0.1:61265. There is a Caddyfile in
[caddy](caddy) you can use if you want to browse
https://symplify-demoapp.localhost.test instead (i.e. using TLS). It needs port
443 free, and your hosts file to be setup of course. This is also required for
cookies to work in this example app.

This example app uses a service created for providing the SDK functionality (see
`services/SymplifyService`). See `Startup.cs` file in the `ConfigureServices` function.

In the index.cshtml we find this snippet
`@Model.client.FindVariation(projectName, Model)` which is the main feature of
the SDK. Model is passed in because it's how the example implements `ICookieJar`
(see [Usage](#Usage)) since it has access to the request and response cookies.

[hosts file]: https://en.wikipedia.org/wiki/Hosts_(file)

## Running CI locally

You can use [act](https://github.com/nektos/act) to execute the GitHub workflow
locally. It requires Docker.

```shell
act
```

## Checklist for Changes

1. pull latest `main`
1. create a new branch for your changes
1. write code and tests
1. add the change to [the changelog](./CHANGELOG.md) under "Unreleased"
1. get the pull request reviewed and approved
1. squash merge the changes to `main`
1. delete the branch that was merged
