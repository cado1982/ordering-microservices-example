FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./Accounting.Tests/Accounting.Tests.csproj ./
COPY ./Accounting/Accounting.csproj ./
RUN dotnet restore ./Accounting.Tests.csproj

COPY . ./
RUN dotnet test ./Accounting.Tests/Accounting.Tests.csproj

ENTRYPOINT [ "dotnet", "test" ]