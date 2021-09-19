# Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["PublicApi/PublicApi.csproj", "./PublicApi/"]
COPY ["ApplicationCore/ApplicationCore.csproj", "./ApplicationCore/"]
COPY ["Infrastructure/Infrastructure.csproj", "./Infrastructure/"]
RUN dotnet restore "PublicApi/PublicApi.csproj"

COPY . .
WORKDIR /app/PublicApi
RUN dotnet build -c Debug -o out
RUN dotnet publish -c Debug -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim
WORKDIR /app
COPY --from=build-env /app/PublicApi/out .
EXPOSE 80
EXPOSE 443
#EXPOSE 64559
#EXPOSE 44313

ENTRYPOINT [ "dotnet", "PublicApi.dll" ]
