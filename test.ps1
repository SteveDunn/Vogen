# Run the end to end tests. The tests can't have project references to Vogen.
# This is because, in Visual Studio, it causes conflicts caused by the difference 
# in runtime; VS uses netstandard2.0
# to load and run the analyzers, but the test project uses a variety of
# runtimes.
# So, it uses NuGet to reference the Vogen analyzer.
# To do this, this script first builds and packs Vogen using a ridiculously high version number
# and then restores the tests NuGet dependencies to use that package.
# This will allow you run and debug debug these tests in VS, but to use any new code
# changes in the analyzer, you need to rerun this script to force a refresh
# of the package. 

$artifacts = ".\artifacts"
$localPackages = ".\local-global-packages"

if(Test-Path $artifacts) { Remove-Item $artifacts -Force -Recurse }

New-Item -Path $artifacts -ItemType Directory

Remove-Item $localPackages\vogen.* -Force

$date1 = Get-Date("2022-10-17");
$date2 = Get-Date;
$patch = [int64]((New-TimeSpan -Start $date1 -End $date2)).TotalSeconds
$version = "999.9." + $patch;

dotnet clean src/Vogen
# dotnet build src/Vogen -c Debug

# dotnet pack -c Debug -o ./artifacts -p ForceVersion=$version --include-source --include-symbols --version-suffix:dev

# Build **just** Vogen first, not the whole solution. If we build the whole
# solution, it'll include the SmallTests, which won't build, because
# because it won't find the Vogen package that we're building in this step

# **NOTE** - we don't want this package ending up in %userprofile%\.nuget\packages
# because, in future builds, NuGet will use this as a cache and won't pick up
# the latest package with the same version number.

dotnet restore ./src/Vogen --packages $localPackages --no-cache

dotnet pack -c Debug -o:$localPackages /p:ForceVersion=$version --include-symbols --version-suffix:dev --no-restore
# dotnet add tests/smalltests package Vogen -v $version

# Restore the project using the custom config file, restoring packages to a local folder
dotnet restore ./tests/SmallTests --force --no-cache --packages $localPackages --configfile: ./nuget.private.config

dotnet build ./tests/SmallTests -c Debug --no-restore

dotnet test ./tests/SmallTests -c Debug --no-build --no-restore 
# dotnet test ./tests/SmallTests -c Debug --no-build --no-restore 

