# Run the end to end tests. The tests can't have project references to Vogen. This is because, in Visual Studio, 
# it causes conflicts caused by the difference in runtime; VS uses netstandard2.0 to load and run the analyzers, but the 
# test project uses a variety of runtimes. So, it uses NuGet to reference the Vogen analyzer. To do this, this script first 
# builds and packs Vogen using a ridiculously high version number and then restores the tests NuGet dependencies to use that
# package. This will allow you run and debug debug these tests in VS, but to use any new code changes in the analyzer, you 
# need to rerun this script to force a refresh of the package. 

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

# Build **just** Vogen first to generate the NuGet package. In the next step,
# we'll build the consumers of package, namely the e2e tests and samples projects.

# **NOTE** - we don't want these 999.9.9.x packages ending up in %userprofile%\.nuget\packages because it'll polute it.

dotnet restore ./src/Vogen --packages $localPackages --no-cache

dotnet pack -c Debug -o:$localPackages /p:ForceVersion=$version --include-symbols --version-suffix:dev --no-restore

# Restore the project using the custom config file, restoring packages to a local folder
dotnet restore ./tests/SmallTests -p UseLocallyBuiltPackage=true --force --no-cache --packages $localPackages --configfile: ./nuget.private.config

dotnet restore ./Samples/Vogen.Examples -p UseLocallyBuiltPackage=true --force --no-cache --packages $localPackages --configfile: ./nuget.private.config

dotnet build ./tests/SmallTests -c Debug --no-restore
dotnet build ./Samples/Vogen.Examples -c Debug --no-restore

dotnet test ./tests/SmallTests -c Debug --no-build --no-restore 


