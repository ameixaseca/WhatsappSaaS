FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WhatsappSaaS.API/WhatsappSaaS.API.csproj", "WhatsappSaaS.API/"]
COPY ["WhatsappSaaS.Infrastructure/WhatsappSaaS.Infrastructure.csproj", "WhatsappSaaS.Infrastructure/"]
COPY ["WhatsappSaaS.Application/WhatsappSaaS.Application.csproj", "WhatsappSaaS.Application/"]
COPY ["WhatsappSaaS.Domain/WhatsappSaaS.Domain.csproj", "WhatsappSaaS.Domain/"]
RUN dotnet restore "WhatsappSaaS.API/WhatsappSaaS.API.csproj"
COPY . .
WORKDIR "/src/WhatsappSaaS.API"
RUN dotnet build "WhatsappSaaS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhatsappSaaS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhatsappSaaS.API.dll"]
