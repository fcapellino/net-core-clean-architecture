FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS base
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["./MainProject.Common/MainProject.Common.csproj","MainProject.Common/"]
COPY ["./MainProject.Domain/MainProject.Domain.csproj","MainProject.Domain/"]
COPY ["./MainProject.Infrastructure/MainProject.Infrastructure.csproj","MainProject.Infrastructure/"]
COPY ["./MainProject.Services/MainProject.Services.csproj","MainProject.Services/"]
COPY ["./MainProject.Web/MainProject.Web.csproj","MainProject.Web/"]
RUN dotnet restore "MainProject.Web/MainProject.Web.csproj"
COPY . .
WORKDIR "/src/MainProject.Web"
RUN dotnet build "MainProject.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MainProject.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","MainProject.Web.dll"]
