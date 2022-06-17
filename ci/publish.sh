#! /bin/sh

#
# Based on pointers in https://acraven.medium.com/a-nuget-package-workflow-using-github-actions-7da8c6557863
#

if [ -z "$VERSION" ]
then
    echo "The environment variable VERSION is required."
    exit 1
fi

if [ -z "$NUGET_API_KEY" ]
then
    echo "The environment variable NUGET_API_KEY is required."
    exit 1
fi

set -e
set -u
set -x

dotnet restore

dotnet build --configuration release /p:Version="$VERSION"

dotnet test --configuration release /p:Version="$VERSION" --no-build

dotnet pack Symplify.Conversion.SDK --configuration Release /p:Version="$VERSION" --no-build --output .

dotnet nuget push ./*"$VERSION.nupkg" --api-key "$NUGET_API_KEY"
