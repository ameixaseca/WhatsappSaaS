FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WhatsappSaaS.Web/WhatsappSaaS.Web.csproj", "WhatsappSaaS.Web/"]
COPY ["WhatsappSaaS.Application/WhatsappSaaS.Application.csproj", "WhatsappSaaS.Application/"]
COPY ["WhatsappSaaS.Domain/WhatsappSaaS.Domain.csproj", "WhatsappSaaS.Domain/"]
RUN dotnet restore "WhatsappSaaS.Web/WhatsappSaaS.Web.csproj"
COPY . .
WORKDIR "/src/WhatsappSaaS.Web"
RUN dotnet build "WhatsappSaaS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhatsappSaaS.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhatsappSaaS.Web.dll"]
