FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
ARG TARGETARCH
WORKDIR /src

COPY ["src/MediaHarbor.Bot/MediaHarbor.Bot.csproj", "MediaHarbor.Bot/"]
COPY ["src/MediaHarbor.Logger/MediaHarbor.Logger.csproj", "MediaHarbor.Logger/"]
COPY ["nuget.config", "nuget/"]
RUN sed -i -e "s/USER/${NUGET_USERNAME}/g" -e "s/PW/${NUGET_PASSWORD}/g" nuget/nuget.config
RUN dotnet restore "MediaHarbor.Bot/MediaHarbor.Bot.csproj" --configfile "nuget/nuget.config" -a $TARGETARCH

COPY . .
WORKDIR "/src/src/MediaHarbor.Bot"
RUN dotnet publish "MediaHarbor.Bot.csproj" -a $TARGETARCH --self-contained false -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MediaHarbor.Bot.dll"]