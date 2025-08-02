FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ContactBook.Reports.Consumer/ContactBook.Reports.Consumer.csproj", "ContactBook.Reports.Consumer/"]
COPY ["src/ContactBook.Reports.Application/ContactBook.Reports.Application.csproj", "ContactBook.Reports.Application/"]
COPY ["src/ContactBook.Reports.Domain/ContactBook.Reports.Domain.csproj", "ContactBook.Reports.Domain/"]
COPY ["src/ContactBook.Reports.Infrastructure/ContactBook.Reports.Infrastructure.csproj", "ContactBook.Reports.Infrastructure/"]
RUN dotnet restore "ContactBook.Reports.Consumer/ContactBook.Reports.Consumer.csproj"
COPY src/ .
WORKDIR "/src/ContactBook.Reports.Consumer"
RUN dotnet build "ContactBook.Reports.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactBook.Reports.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactBook.Reports.Consumer.dll"]
