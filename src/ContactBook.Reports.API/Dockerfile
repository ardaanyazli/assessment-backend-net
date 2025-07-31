FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ContactBook.Reports.API/ContactBook.Reports.API.csproj", "ContactBook.Reports.API/"]
COPY ["src/ContactBook.Reports.Application/ContactBook.Reports.Application.csproj", "ContactBook.Reports.Application/"]
COPY ["src/ContactBook.Reports.Domain/ContactBook.Reports.Domain.csproj", "ContactBook.Reports.Domain/"]
COPY ["src/ContactBook.Reports.Infrastructure/ContactBook.Reports.Infrastructure.csproj", "ContactBook.Reports.Infrastructure/"]
RUN dotnet restore "ContactBook.Reports.API/ContactBook.Reports.API.csproj"
COPY src/ .
WORKDIR "/src/ContactBook.Reports.API"
RUN dotnet build "ContactBook.Reports.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactBook.Reports.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactBook.Reports.API.dll"]
