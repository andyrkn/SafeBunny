#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SafeBunny.Consumer.Delivery/SafeBunny.Consumer.Delivery.csproj", "SafeBunny.Consumer.Delivery/"]
COPY ["SafeBunny.Domain/SafeBunny.Domain.csproj", "SafeBunny.Domain/"]
COPY ["SafeBunny.Hosts/SafeBunny.Hosts.csproj", "SafeBunny.Hosts/"]
COPY ["SafeBunny.SignalR/SafeBunny.SignalR.csproj", "SafeBunny.SignalR/"]
COPY ["SafeBunny.SignalR.HubResources/SafeBunny.SignalR.HubResources.csproj", "SafeBunny.SignalR.HubResources/"]
RUN dotnet nuget add source "https://pkgs.dev.azure.com/ratzoiu2000/_packaging/ratzoiu2000/nuget/v3/index.json"
RUN dotnet restore "SafeBunny.Consumer.Delivery/SafeBunny.Consumer.Delivery.csproj"
COPY . .
WORKDIR "/src/SafeBunny.Consumer.Delivery"
RUN dotnet build "SafeBunny.Consumer.Delivery.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SafeBunny.Consumer.Delivery.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SafeBunny.Consumer.Delivery.dll"]
