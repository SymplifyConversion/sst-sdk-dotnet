#! /bin/sh

set -eu
set -x

dotnet test --configuration Release -f net6.0 --verbosity normal --no-restore
