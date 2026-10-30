FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["IngestionEngine.Api.csproj", "./"]
RUN dotnet restore "IngestionEngine.Api.csproj"

COPY . .
RUN dotnet publish "IngestionEngine.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "IngestionEngine.Api.dll"]