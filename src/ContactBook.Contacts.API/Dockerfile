FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ContactBook.Contacts.API/ContactBook.Contacts.API.csproj", "ContactBook.Contacts.API/"]
COPY ["src/ContactBook.Contacts.Application/ContactBook.Contacts.Application.csproj", "ContactBook.Contacts.Application/"]
COPY ["src/ContactBook.Contacts.Domain/ContactBook.Contacts.Domain.csproj", "ContactBook.Contacts.Domain/"]
COPY ["src/ContactBook.Contacts.Infrastructure/ContactBook.Contacts.Infrastructure.csproj", "ContactBook.Contacts.Infrastructure/"]
RUN dotnet restore "ContactBook.Contacts.API/ContactBook.Contacts.API.csproj"
COPY src/ .
WORKDIR "/src/ContactBook.Contacts.API"
RUN dotnet build "ContactBook.Contacts.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactBook.Contacts.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactBook.Contacts.API.dll"]
