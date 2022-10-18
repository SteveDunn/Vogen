param($verbosity = "minimal") #quite|q, minimal|m, normal|n, detailed|d

$artifacts = ".\artifacts"

function WriteStage([string]$message)
{
    Write-Host "############################################" -ForegroundColor Green
    Write-Host $message -ForegroundColor Yellow
    Write-Output ""
    Write-Output ""
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
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

WriteStage("Building release version of Vogen...")

if(Test-Path $artifacts) { Remove-Item $artifacts -Force -Recurse }
New-Item -Path $artifacts -ItemType Directory

exec { & dotnet clean Vogen.sln -c Release --verbosity $verbosity}
exec { & dotnet clean Consumers.sln -c Release --verbosity $verbosity}

exec { & dotnet build Vogen.sln -c Release --verbosity $verbosity}

WriteStage("Running analyzer and code generation tests...")


# run the analyzer and code generation tests
exec { & dotnet test Vogen.sln -c Release --no-build -l trx -l "GitHubActions;report-warnings=false" --verbosity $verbosity }


################################################################


# Run the end to end tests. The tests can't have project references to Vogen. This is because, in Visual Studio, 
# it causes conflicts caused by the difference in runtime; VS uses netstandard2.0 to load and run the analyzers, but the 
# test project uses a variety of runtimes. So, it uses NuGet to reference the Vogen analyzer. To do this, this script first 
# builds and packs Vogen using a ridiculously high version number and then restores the tests NuGet dependencies to use that
# package. This will allow you run and debug debug these tests in VS, but to use any new code changes in the analyzer, you 
# need to rerun this script to force a refresh of the package. 

WriteStage("Building NuGet for local version of Vogen that will be used to run end to end tests and samples...")


$localPackages = ".\local-global-packages"

if(Test-Path $localPackages) { Remove-Item $localPackages\vogen.* -Force -ErrorAction SilentlyContinue }

$date1 = Get-Date("2022-10-17");
$date2 = Get-Date;
$patch = [int64]((New-TimeSpan -Start $date1 -End $date2)).TotalSeconds
$version = "999.9." + $patch;


# dotnet clean src/Vogen

# Build **just** Vogen first to generate the NuGet package. In the next step,
# we'll build the consumers of package, namely the e2e tests and samples projects.

# **NOTE** - we don't want these 999.9.9.x packages ending up in %userprofile%\.nuget\packages because it'll polute it.

dotnet restore ./src/Vogen --packages $localPackages --no-cache --verbosity $verbosity

dotnet pack ./src/Vogen -c Debug -o:$localPackages /p:ForceVersion=$version --include-symbols --version-suffix:dev --no-restore --verbosity $verbosity

# Restore the project using the custom config file, restoring packages to a local folder
dotnet restore ./tests/SmallTests -p UseLocallyBuiltPackage=true --force --no-cache --packages $localPackages --configfile: ./nuget.private.config --verbosity $verbosity

dotnet restore ./Samples/Vogen.Examples -p UseLocallyBuiltPackage=true --force --no-cache --packages $localPackages --configfile: ./nuget.private.config --verbosity $verbosity

dotnet build ./tests/SmallTests -c Debug --no-restore --verbosity $verbosity
dotnet build ./Samples/Vogen.Examples -c Debug --no-restore --verbosity $verbosity

WriteStage("Running end to end tests with the local version of the NuGet package:" +$version)

dotnet test ./tests/SmallTests -c Debug --no-build --no-restore --verbosity $verbosity


WriteStage("Building samples using the local version of the NuGet package...")


$samples = Get-ChildItem .\samples -Directory

WriteStage("Samples found are " + $samples)

foreach ($sample in $samples) {
    Push-Location -Path .\samples\$sample

    WriteStage("Running sample " + $sample)


    try {
        exec { & dotnet run -c Debug --no-build --no-restore }
    } catch {
    } finally {
        Pop-Location
    }
}

WriteStage("Finally, packing the release version into " + $artifacts)


exec { & dotnet pack src/Vogen -c Release -o $artifacts --no-build --verbosity $verbosity }

