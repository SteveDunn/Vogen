param($verbosity = "minimal", [switch] $reset = $false) #quite|q, minimal|m, normal|n, detailed|d

$artifacts = ".\artifacts"
$localPackages = ".\local-global-packages"

function WriteStage([string]$message)
{
    Write-Host "############################################" -ForegroundColor Cyan
    Write-Host "**** " $message -ForegroundColor Cyan
    Write-Host "############################################" -ForegroundColor Cyan
    Write-Output ""
}

function Remove-SnapshotsFolders($path) {
    $folders = Get-ChildItem -Path $path -Directory -Filter "snapshots" -Recurse
    
    foreach ($folder in $folders) {
        Write-Host "Deleting folder: $($folder.FullName)"
        Remove-Item -Path $folder.FullName -Recurse -Force
    }
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

if($reset) 
{
    WriteStage("Resetting snapshots...")
    Remove-SnapshotsFolders(".\");
}

WriteStage("Cleaning, restoring, and building release version of Vogen...")

WriteStage("... clean ...")
exec { & dotnet clean Vogen.sln -c Release --verbosity $verbosity}

WriteStage("... restore ...")
exec { & dotnet restore Vogen.sln --no-cache --verbosity $verbosity }

if($reset)
{
    WriteStage("... resetting snapshots ...")
    exec { & dotnet build Vogen.sln -c Release -p Thorough=true -p ResetSnapshots=true --no-restore --verbosity $verbosity}
}
else
{
    exec { & dotnet build Vogen.sln -c Release -p Thorough=true --no-restore --verbosity $verbosity}
}

################################################################
WriteStage("Running snapshot tests...")
exec { & dotnet test tests/SnapshotTests/SnapshotTests.csproj -c Release --no-build -l trx -l "GitHubActions;report-warnings=false" --verbosity $verbosity }
################################################################

WriteStage("Done!")
