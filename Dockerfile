FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7262

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Project.WebApi/Project.WebApi.csproj", "Project.WebApi/"]
COPY ["Project.Common.Services/Project.Common.Services.csproj", "Project.Common.Services/"]
COPY ["Project.Common.DataAccess/Project.Common.DataAccess.csproj", "Project.Common.DataAccess/"]
RUN dotnet restore "Project.WebApi/Project.WebApi.csproj"
COPY . .

WORKDIR "/src/Project.WebApi"
RUN dotnet build "Project.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Project.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project.WebApi.dll"]
