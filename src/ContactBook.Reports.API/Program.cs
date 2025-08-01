using ContactBook.Reports.Application.Interfaces;
using ContactBook.Reports.Infrastructure.Persistence;
using ContactBook.Reports.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Configure database connection
builder.Services.AddDbContext<ReportDbContext>(options =>{
    options.UseNpgsql(builder.Configuration.GetConnectionString("reportsDb"));
    });
    builder.Services.AddScoped<IReportRepository,ReportRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
            db.Database.Migrate(); // Ensures DB and schema exist
        }

}

app.UseHttpsRedirection();

app.MapControllers();
app.UseHealthChecks("/health");
app.Run();

