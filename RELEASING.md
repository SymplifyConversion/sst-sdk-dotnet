## Checklist for Releases

We practice [trunk based development](https://trunkbaseddevelopment.com) and
`main` is the branch we release from.

1. pull latest `main`
1. review "Unreleased" in [the changelog](./CHANGELOG.md) to decide if
   the release is a major, minor, or patch release `vX.Y.Z`
1. create a new branch `release/vX.Y.Z` matching the version
1. update links and headings in [the changelog](./CHANGELOG.md) to reflect the new version
1. open a pull request with your release branch
1. get the pull request reviewed and approved
1. push the branch, then squash merge your changes to `main`
1. tag the merged commit `vX.Y.Z` and push the tag (CI should pick this up and publish the version)
1. [create a matching GitHub release](https://github.com/SymplifyConversion/sst-sdk-dotnet/releases/new)

## Pre-releases

Prerelease versions can also be published as long as the commit is in
`origin/main`. Tag the commit `vX.Y.Z-previewNN` where NN is a number with
leading zeros (for sorting properly) and push the tag.
