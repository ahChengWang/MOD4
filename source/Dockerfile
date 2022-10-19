 FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
 WORKDIR /app
 FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
 WORKDIR /src
 COPY ["MOD4.Web.csproj", ""]
 RUN dotnet restore "./MOD4.Web.csproj"
 COPY . .
 WORKDIR "/src/."
 RUN dotnet build "MOD4.Web.csproj" -c Release -o /app/build
 FROM build AS publish
 RUN dotnet publish "MOD4.Web.csproj" -c Release -o /app/publish
 FROM base AS final
 WORKDIR /app
 COPY --from=publish /app/publish .
 ENTRYPOINT ["dotnet", "MOD4.Web.dll"]