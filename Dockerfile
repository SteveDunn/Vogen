FROM mcr.microsoft.com/dotnet/sdk:7.0

# SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]

COPY . .

ENTRYPOINT [ "powershell.exe", "./Build.ps1" ]
