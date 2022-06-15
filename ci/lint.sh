#! /bin/sh

set -eu
set -x

dotnet format --no-restore --verify-no-changes
dotnet build --no-restore --configuration release
