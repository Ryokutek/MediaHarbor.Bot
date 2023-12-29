FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build

ARG NUGET_USERNAME
ARG NUGET_PASSWORD

WORKDIR /src
COPY ["src/MediaHarbor.Bot/MediaHarbor.Bot.csproj", "MediaHarbor.Bot/"]
COPY ["src/MediaHarbor.Logger/MediaHarbor.Logger.csproj", "MediaHarbor.Logger/"]
COPY ["nuget.config", "nuget/"]
RUN sed -i -e "s/USER/${NUGET_USERNAME}/g" -e "s/PW/${NUGET_PASSWORD}/g" nuget/nuget.config
RUN dotnet restore "MediaHarbor.Bot/MediaHarbor.Bot.csproj" --configfile "nuget/nuget.config"
COPY . .
WORKDIR "/src/src/MediaHarbor.Bot"
RUN dotnet build "MediaHarbor.Bot.csproj" -c Release -o /app/build

FROM backend-build AS publish
RUN dotnet publish "MediaHarbor.Bot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaHarbor.Bot.dll"]