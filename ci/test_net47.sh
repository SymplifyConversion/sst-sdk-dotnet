#! /bin/sh

set -u
set -x

if nuget help locals >/dev/null
then
    NUGET_PACKAGES_DIR=$(nuget locals global-packages -list | cut -d' ' -f2)
else
    NUGET_PACKAGES_DIR=~/.nuget/packages
    echo "nuget command 'locals' is not available, assuming global packages dir is: $NUGET_PACKAGES_DIR"
fi

set -e

XUNIT_VERSION=2.4.2
PROJECT=Symplify.Conversion.SDK.Tests
CONFIGURATION=Release

# the nuget version in Ubuntu does not provide a "locals" command, so we assume the path
XUNIT_TOOLS_DIR="$NUGET_PACKAGES_DIR"/xunit.runner.console/"$XUNIT_VERSION"/tools
XUNIT_RUNNER_EXE="$XUNIT_TOOLS_DIR"/net472/xunit.console.exe

dotnet build --configuration "$CONFIGURATION" --no-restore
mono "$XUNIT_RUNNER_EXE" "$PROJECT"/bin/"$CONFIGURATION"/net47/"$PROJECT".dll
