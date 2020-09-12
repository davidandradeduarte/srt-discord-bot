FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SimpleRandomTeams/SimpleRandomTeams.csproj", "SimpleRandomTeams/"]
RUN dotnet restore "SimpleRandomTeams/SimpleRandomTeams.csproj"
COPY . .
WORKDIR "/src/SimpleRandomTeams"
RUN dotnet build "SimpleRandomTeams.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleRandomTeams.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleRandomTeams.dll"]