FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NextDnsMcp.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV NEXTDNS_API_KEY=""
ENV NEXTDNS_PROFILE_ID=""

ENTRYPOINT ["dotnet", "NextDnsMcp.dll"]
