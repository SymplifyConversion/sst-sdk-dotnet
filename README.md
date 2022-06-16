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

NuGet package Coming soon...

## Usage

On your application startup:

1. Create an SDK instance.
2. Call LoadConfig to start its config updater loop.

On each request where you want to use variations:

1. Call findVariation, providing a `CookieJar` (see below)

See example usage in the project `SymplifySDK.DemoApp` (and instructions in [CONTRIBUTING.md](CONTRIBUTING.md))

### CookieJar

In order to assign variations stably in face of configuration changes, we need
to persist the allocation for each user somehow. The SST SDK uses cookies for
this (which also helps integration with the frontend js-sdk).

To be compatible with any web framework, the SDK uses an interface `ICookieJar`
which you implement to provide reading and writing of cookies.

## SDK Development

See [CONTRIBUTING.md](CONTRIBUTING.md) or [RELEASING.md](RELEASING.md).
