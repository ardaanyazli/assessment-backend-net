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


app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ContactBook Reports API V1");
    });
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ReportDbContext>();
        db.Database.Migrate(); // Ensures DB and schema exist
    }
}
app.Run();

