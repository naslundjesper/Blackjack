FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiera allt
COPY . .

# Bygg exakt Blackjack.csproj
RUN dotnet restore Blackjack.csproj
RUN dotnet publish Blackjack.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Blackjack.dll"]
