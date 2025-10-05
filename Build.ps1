param($verbosity = "minimal", [switch] $quick = $false) #quite|q, minimal|m, normal|n, detailed|d

$artifacts = ".\artifacts"
$localPackages = ".\local-global-packages"

function WriteStage([string]$message)
{
    Write-Host "############################################" -ForegroundColor Cyan
    Write-Host "**** " $message -ForegroundColor Cyan
    Write-Host "############################################" -ForegroundColor Cyan
    Write-Output ""
}

function BuildWith([string]$configuration)
{
    WriteStage("... building " + $configuration)

    exec { & dotnet build Vogen.sln -c $configuration -p Thorough=true --no-restore --verbosity $verbosity}
    exec { & dotnet build src/Vogen/Vogen.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.4 --verbosity $verbosity}
    exec { & dotnet build src/Vogen/Vogen.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.6 --verbosity $verbosity}
    exec { & dotnet build src/Vogen/Vogen.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.8 --verbosity $verbosity}
    exec { & dotnet build src/Vogen/Vogen.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.11 --verbosity $verbosity}
    exec { & dotnet build src/Vogen/Vogen.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.12 --verbosity $verbosity}

    exec { & dotnet build src/Vogen.CodeFixers/Vogen.CodeFixers.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.4 --verbosity $verbosity}
    exec { & dotnet build src/Vogen.CodeFixers/Vogen.CodeFixers.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.6 --verbosity $verbosity}
    exec { & dotnet build src/Vogen.CodeFixers/Vogen.CodeFixers.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.8 --verbosity $verbosity}
    exec { & dotnet build src/Vogen.CodeFixers/Vogen.CodeFixers.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.11 --verbosity $verbosity}
    exec { & dotnet build src/Vogen.CodeFixers/Vogen.CodeFixers.csproj -c $configuration -p Thorough=true -p RoslynVersion=roslyn4.12 --verbosity $verbosity}
}

function Get999VersionWithUniquePatch()
{
    $date1 = Get-Date("2022-10-17");
    $date2 = Get-Date;
    $patch = [int64]((New-TimeSpan -Start $date1 -End $date2)).TotalSeconds
    return "999.9." + $patch;
}

<#
.SYNOPSIS
  Taken from psake https://github.com/psake/psake
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = "Error executing command $cmd"
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

WriteStage("Building release version of Vogen...")

if(Test-Path $artifacts) { Remove-Item $artifacts -Force -Recurse }

New-Item -Path $artifacts -ItemType Directory

New-Item -Path $localPackages -ItemType Directory -ErrorAction SilentlyContinue

if(Test-Path $localPackages) { Remove-Item $localPackages\vogen.* -Force -ErrorAction SilentlyContinue }

WriteStage("Cleaning, restoring, and building release version of Vogen...")

WriteStage("... clean ...")
exec { & dotnet clean Vogen.sln -c Release --verbosity $verbosity}

WriteStage("... restore ...")
exec { & dotnet restore Vogen.sln --no-cache --verbosity $verbosity }

BuildWith("Release");

# run the analyzer tests
WriteStage("Running analyzer tests...")
exec { & dotnet test tests/AnalyzerTests/AnalyzerTests.csproj -c Release --no-build -l trx -l "GitHubActions;report-warnings=false" --verbosity $verbosity }

WriteStage("Running unit tests...")
exec { & dotnet test tests/Vogen.Tests/Vogen.Tests.csproj -c Release --no-build -l trx -l "GitHubActions;report-warnings=false" --verbosity $verbosity }
    
# Run the end to end tests. The tests can't have project references to Vogen. This is because, in Visual Studio, 
# it causes conflicts caused by the difference in runtime; VS uses netstandard2.0 to load and run the analyzers, but the 
# test project uses a variety of runtimes. So, it uses NuGet to reference the Vogen analyzer. To do this, this script first 
# builds and packs Vogen using a ridiculously high version number and then restores the tests NuGet dependencies to use that
# package. This will allow you run and debug these tests in VS, but to use any new code changes in the analyzer, you 
# need to rerun this script to force a refresh of the package. 

WriteStage("Building NuGet for local version of Vogen that will be used to run end to end tests and samples...")

$version = Get999VersionWithUniquePatch

# Build **just** Vogen first to generate the NuGet package. In the next step,
# we'll build the consumers of package, namely the e2e tests and samples projects.

# **NOTE** - we don't want these 999.9.9.x packages ending up in %userprofile%\.nuget\packages because it'll polute it.

exec { & dotnet restore Vogen.sln --packages $localPackages --no-cache --verbosity $verbosity }

BuildWith("Debug");

exec { & dotnet pack ./src/Vogen.Pack/Vogen.Pack.csproj -c Debug -o:$localPackages /p:ForceVersion=$version --include-symbols --version-suffix:dev --no-restore --verbosity $verbosity }

WriteStage("Cleaning and building consumers (tests and samples) - verbosity of $verbosity")

exec { & dotnet restore Consumers.sln --no-cache --verbosity $verbosity }
exec { & dotnet clean Consumers.sln -c Release --verbosity $verbosity}


# Restore the project using the custom config file, restoring packages to a local folder
exec { & dotnet restore Consumers.sln -p UseLocallyBuiltPackage=true --force --no-cache --packages $localPackages --configfile ./nuget.private.config --verbosity $verbosity }

# Run both build tasks asynchronously

$debugTask = Start-Process "dotnet" "build Consumers.sln --configuration Debug --no-restore --verbosity $verbosity" -NoNewWindow -PassThru
$releaseTask = Start-Process "dotnet" "build Consumers.sln --configuration Release --no-restore --verbosity $verbosity" -NoNewWindow -PassThru

# Wait for both tasks to complete
$debugTask.WaitForExit()
$releaseTask.WaitForExit()

if ($null -ne $debugTask.ExitCode -and $debugTask.ExitCode -ne 0) {
    Write-Host "debug build returned " + $debugTask.ExitCode
    exit -1
}
if ($null -ne $releaseTask.ExitCode -and $releaseTask.ExitCode -ne 0) {
    Write-Host "release build returned " + $releaseTask.ExitCode
    exit -1
}


#exec { & dotnet build Consumers.sln -c Debug --no-restore --verbosity $verbosity }
#exec { & dotnet build Consumers.sln -c Release --no-restore --verbosity $verbosity }

WriteStage("Running consumer tests in debug with the local version of the NuGet package:" +$version)
exec { & dotnet test ./tests/ConsumerTests -c Debug --no-build --no-restore --verbosity $verbosity }

if(-not $quick)
{
    WriteStage("Re-running tests in release with the local version of the NuGet package:" + $version)
    exec { & dotnet test ./tests/ConsumerTests -c Release --no-build --no-restore --verbosity $verbosity }

    WriteStage("Re-running tests in release with no validation with the local version of the NuGet package:" +$version)
    exec { & dotnet build Consumers.sln -c Release -p:DefineConstants="VOGEN_NO_VALIDATION" --no-restore --verbosity $verbosity }
    exec { & dotnet test ./tests/ConsumerTests -c Release --no-build --no-restore --verbosity $verbosity }

    WriteStage("Building and running samples using the local version of the NuGet package...")
    exec { & dotnet run --project samples/Vogen.Examples/Vogen.Examples.csproj -c Debug --no-build  --no-restore }
    exec { & dotnet run --project samples/Vogen.Examples/Vogen.Examples.csproj -c Release --no-build --no-restore }
}

WriteStage("Finally, packing the release version into " + $artifacts)
exec { & dotnet pack src/Vogen.Pack/Vogen.Pack.csproj -c Release -o $artifacts --no-build --verbosity $verbosity }

WriteStage("Done! Package generated at " + $artifacts)
