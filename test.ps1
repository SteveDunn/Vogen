$artifacts = ".\local-packages"

if(Test-Path $artifacts) { Remove-Item $artifacts -Force -Recurse }

# Restore the project using the custom config file, restoring packages to a local folder

dotnet clean -c Debug

dotnet restore ./src/vogen --packages ./local-packages --configfile "nuget.local-packages.config" 

# Build the project (no restore), using the packages restored to the local folder
dotnet build ./src/vogen -c Debug --packages ./local-packages --no-restore

dotnet build ./src/vogen NugetIntegrationTests -c Release --packages ./packages --no-restore

# Test the project (no build or restore)
dotnet test ./tests/NetEscapades.EnumGenerators.NugetIntegrationTests -c Release --no-build --no-restore 


exec { & dotnet clean -c Debug }

exec { & dotnet clean -c Debug }

exec { & dotnet restore ./tests/SmallTests --packages ./packages --configfile "nuget.local-packages.config" }
