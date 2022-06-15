#! /bin/sh

set -eu
set -x

dotnet format --verify-no-changes
dotnet build --configuration release
