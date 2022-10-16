FROM mcr.microsoft.com/dotnet/sdk:7.0


COPY . .

tactic: copy everything over, clean it, build vogen, pack vogen to another folder
restore small tests, using that folder - it should then pick
up the latest
That way, we don't have to auto-pack every build on the host.
We still need a way of installign multiple SDKs - maybe we could have
multiple docker images for each SDK?

RUN dotnet clean
RUN dotnet restore
RUN dotnet build 

ADD ./tests/SmallTests ./smalltests
ADD ./Directory.Build.props ./smalltests
ADD ./artifacts ./smalltests/artifacts
COPY ./nuget.config ./smalltests


# COPY ./tests/smalltests/ .

#  COPY ../../artifacts local-packages
# WORKDIR /app/local-packages

WORKDIR smalltests

RUN dotnet restore # --configfile nuget.local-packages.config

RUN bash #dotnet test

ENTRYPOINT bash
# ENTRYPOINT ["dotnet", "test"]
