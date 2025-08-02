# ContactBook Solution

A comprehensive contact management system built with .NET 9, featuring microservices architecture, event-driven communication, and comprehensive reporting capabilities.

## 🏗️ Architecture Overview

This solution follows Clean Architecture principles and implements a microservices pattern with the following components:

### Core Services
- **ContactBook.Contacts.API** - RESTful API for contact management
- **ContactBook.Reports.API** - RESTful API for report generation and retrieval
- **ContactBook.Reports.Consumer** - Background service for processing report requests

### Architecture Layers
- **Domain Layer** - Contains business entities and domain logic
- **Application Layer** - Contains interfaces, DTOs, and application services
- **Infrastructure Layer** - Contains data access, external services, and persistence
- **API Layer** - Contains controllers, middleware, and API configuration

## 📁 Project Structure

```
ContactBook/
├── src/
│   ├── ContactBook.Contacts.API/          # Contacts REST API
│   ├── ContactBook.Contacts.Application/   # Contacts business logic
│   ├── ContactBook.Contacts.Domain/        # Contacts domain entities
│   ├── ContactBook.Contacts.Infrastructure/ # Contacts data access
│   ├── ContactBook.Reports.API/            # Reports REST API
│   ├── ContactBook.Reports.Application/    # Reports business logic
│   ├── ContactBook.Reports.Domain/         # Reports domain entities
│   ├── ContactBook.Reports.Infrastructure/ # Reports data access
│   └── ContactBook.Reports.Consumer/       # Background report processor
├── tests/
│   ├── ContactBook.Contacts.API.Tests/     # API unit tests
│   ├── ContactBook.Reports.API.Tests/      # Reports API tests
│   ├── ContactBook.Infrastructure.Tests/   # Infrastructure tests
│   ├── ContactBook.Domain.Tests/           # Domain entity tests
│   └── ContactBook.Integration.Tests/      # Integration tests
└── docker-compose.yml                      # Container orchestration
```

## 🚀 Features

### Contact Management
- ✅ Create, read, update, and delete contacts
- ✅ Manage contact information (email, phone, location)
- ✅ Set default contact information
- ✅ Comprehensive validation and error handling
- ✅ Cancellation token support for all operations

### Reporting System
- ✅ Asynchronous report generation
- ✅ Location-based contact statistics
- ✅ Phone number distribution by location
- ✅ Event-driven architecture using Apache Kafka
- ✅ Report status tracking (Requested → InProgress → Completed)

### Technical Features
- ✅ Clean Architecture with DDD principles
- ✅ Entity Framework Core with PostgreSQL
- ✅ Apache Kafka for message broker
- ✅ Comprehensive error handling and logging
- ✅ Request cancellation support
- ✅ Health checks for monitoring
- ✅ OpenAPI/Swagger documentation
- ✅ Docker containerization

## 🛠️ Technology Stack

- **Framework**: .NET 9
- **Database**: PostgreSQL
- **Message Broker**: Apache Kafka
- **ORM**: Entity Framework Core
- **API Documentation**: OpenAPI/Swagger
- **Testing**: xUnit, Moq, FluentAssertions
- **Containerization**: Docker & Docker Compose

## 📋 Prerequisites

- .NET 9 SDK
- Docker and Docker Compose
- PostgreSQL (or use Docker)
- Apache Kafka (or use Docker)

## 🏃‍♂️ Getting Started

### 1. Clone the Repository
```bash
git clone <repository-url>
cd ContactBook
```

### 2. Start Infrastructure Services
```bash
# Start PostgreSQL and Kafka using Docker Compose
docker-compose up -d postgres kafka zookeeper
```

### 3. Update Connection Strings
Update the connection strings in `appsettings.Development.json` files:

**ContactBook.Contacts.API/appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "contactsDb": "Host=localhost;Port=5432;Database=ContactsBook;Username=postgres;Password=postgres"
  }
}
```

**ContactBook.Reports.API/appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "reportsDb": "Host=localhost;Port=5432;Database=ContactBookReports;Username=postgres;Password=postgres"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}
```

### 4. Run Database Migrations
```bash
# Contacts Database
cd src/ContactBook.Contacts.API
dotnet ef database update

# Reports Database
cd ../ContactBook.Reports.API
dotnet ef database update
```

### 5. Start the Services

#### Option A: Using Docker Compose (Recommended)
```bash
docker-compose up --build
```

#### Option B: Run Individually
```bash
# Terminal 1 - Contacts API
cd src/ContactBook.Contacts.API
dotnet run

# Terminal 2 - Reports API
cd src/ContactBook.Reports.API
dotnet run

# Terminal 3 - Reports Consumer
cd src/ContactBook.Reports.Consumer
dotnet run
```

### 6. Access the APIs
- **Contacts API**: http://localhost:5115
- **Contacts Swagger**: http://localhost:5115/swagger
- **Reports API**: http://localhost:5263
- **Reports Swagger**: http://localhost:5263/swagger

## 📖 API Usage Examples

### Contact Management

#### Create a Contact
```bash
curl -X POST "http://localhost:5115/Contacts" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "contactInfo": [
      {
        "contactInfoType": 2,
        "value": "john.doe@example.com",
        "isDefault": true
      },
      {
        "contactInfoType": 1,
        "value": "+1-555-123-4567",
        "isDefault": false
      },
      {
        "contactInfoType": 3,
        "value": "Istanbul, Turkey",
        "isDefault": true
      }
    ]
  }'
```

#### Get All Contacts
```bash
curl -X GET "http://localhost:5115/Contacts"
```

#### Get Contact by ID
```bash
curl -X GET "http://localhost:5115/Contacts/{contact-id}"
```

#### Update Contact
```bash
curl -X PUT "http://localhost:5115/Contacts/{contact-id}" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Smith"
  }'
```

#### Delete Contact
```bash
curl -X DELETE "http://localhost:5115/Contacts/{contact-id}"
```

### Report Management

#### Request a Report
```bash
curl -X POST "http://localhost:5263/Reports"
```

#### Get All Reports
```bash
curl -X GET "http://localhost:5263/Reports"
```

#### Get Specific Report
```bash
curl -X GET "http://localhost:5263/Reports/{report-id}"
```

## 🧪 Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Project
```bash
# Unit tests for Contacts API
dotnet test tests/ContactBook.Contacts.API.Tests/

# Integration tests
dotnet test tests/ContactBook.Integration.Tests/

# Infrastructure tests
dotnet test tests/ContactBook.Infrastructure.Tests/
```

### Generate Coverage Report
```bash
# Install reportgenerator tool
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
```

## 🏗️ Development Workflow

### Adding a New Contact Information Type

1. **Update Domain Entity**:
```csharp
// ContactBook.Contacts.Domain/Entities/ContactInfoType.cs
public enum ContactInfoType
{
    Phone = 1,
    Email = 2,
    Location = 3,
    Website = 4  // New type
}
```

2. **Create and Run Migration**:
```bash
cd src/ContactBook.Contacts.API
dotnet ef migrations add AddWebsiteContactType
dotnet ef database update
```

3. **Update Controller Logic** (if needed):
```csharp
// Handle the new contact info type in validation/parsing logic
```

### Adding a New Report Type

1. **Update Domain Models**:
```csharp
// Add new report data structures in ContactBook.Reports.Domain
```

2. **Update Consumer Logic**:
```csharp
// Modify ReportWorker.cs to generate new report types
```

3. **Update API Responses**:
```csharp
// Update DTOs and controller actions
```

## 🔧 Configuration

### Environment Variables

#### Contacts API
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__contactsDb`: PostgreSQL connection string

#### Reports API
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__reportsDb`: PostgreSQL connection string
- `Kafka__BootstrapServers`: Kafka broker address

#### Reports Consumer
- `ConnectionStrings__contactsDb`: Contacts database connection
- `ConnectionStrings__reportsDb`: Reports database connection
- `Kafka__BootstrapServers`: Kafka broker address
- `Kafka__ConsumerGroupId`: Kafka consumer group ID

### Docker Configuration

The included `docker-compose.yml` provides:
- PostgreSQL database
- Apache Kafka with Zookeeper
- All application services
- Health checks and service dependencies

## 🔍 Monitoring and Health Checks

### Health Check Endpoints
- **Contacts API**: `GET /health`
- **Reports API**: `GET /health`

### Logging
All services use structured logging with:
- Request/response logging
- Error logging with correlation IDs
- Performance metrics
- Kafka message processing logs

### Metrics
Monitor the following metrics:
- API response times
- Database query performance
- Kafka message processing latency
- Error rates by endpoint

## 🚨 Error Handling

The solution implements comprehensive error handling:

### Global Exception Handling
- `ErrorHandlingMiddleware`: Catches unhandled exceptions
- `RequestCancellationMiddleware`: Handles request cancellations
- Structured error responses with correlation IDs

### Validation
- Model validation using Data Annotations
- Business rule validation in domain services
- Proper HTTP status codes for different scenarios

### Resilience Patterns
- Request cancellation support
- Graceful degradation
- Retry policies for external services

## 🔒 Security Considerations

### Current Implementation
- Input validation and sanitization
- SQL injection prevention via EF Core
- Request size limits
- CORS configuration

### Recommended Enhancements
- Authentication and authorization (JWT/OAuth)
- Rate limiting
- Input encryption for sensitive data
- Audit logging
- HTTPS enforcement in production

## 📈 Performance Optimization

### Database Optimization
- Proper indexing on foreign keys
- Query optimization with EF Core
- Connection pooling
- Read/write separation potential

### Caching Strategy
- In-memory caching for frequently accessed data
- Distributed caching with Redis
- Cache invalidation on data updates

### Scaling Considerations
- Horizontal scaling with multiple instances
- Database sharding by location
- Message broker partitioning
- CDN for static content

## 🐳 Docker Deployment

### Production Docker Compose
```yaml
version: '3.8'
services:
  contacts-api:
    image: contactbook/contacts-api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__contactsDb=Host=postgres;Database=ContactsBook;Username=postgres;Password=${POSTGRES_PASSWORD}
    depends_on:
      - postgres
      
  reports-api:
    image: contactbook/reports-api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__reportsDb=Host=postgres;Database=ContactBookReports;Username=postgres;Password=${POSTGRES_PASSWORD}
      - Kafka__BootstrapServers=kafka:9092
    depends_on:
      - postgres
      - kafka
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Standards
- Follow C# coding conventions
- Write unit tests for new features
- Maintain 60%+ code coverage
- Update documentation for API changes
- Use conventional commit messages

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Troubleshooting

### Common Issues

#### Database Connection Issues
```bash
# Check if PostgreSQL is running
docker ps | grep postgres

# Check database connectivity
docker exec -it postgres_container psql -U postgres -l
```

#### Kafka Connection Issues
```bash
# Check Kafka logs
docker logs kafka_container

# List topics
docker exec -it kafka_container kafka-topics --list --bootstrap-server localhost:9092
```

#### Migration Issues
```bash
# Reset migrations (development only)
dotnet ef database drop
dotnet ef database update
```

### Debug Configuration
Enable detailed logging in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 📞 Support

For support and questions:
- Create an issue in the repository
- Check existing documentation
- Review test cases for usage examples

## 🔄 Version History

- **v1.0.0** - Initial release with core functionality
  - Contact CRUD operations
  - Async report generation
  - Event-driven architecture
  - Comprehensive testing suite