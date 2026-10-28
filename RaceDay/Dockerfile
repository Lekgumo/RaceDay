FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RaceDay/RaceDay.csproj", "RaceDay/"]
RUN dotnet restore "RaceDay/RaceDay.csproj"
COPY . .
WORKDIR "/src/RaceDay"
RUN dotnet build "RaceDay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RaceDay.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RaceDay.dll"]
