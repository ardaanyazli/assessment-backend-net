using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Contacts.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ContactsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("contactsDb")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

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
        options.SwaggerEndpoint("/openapi/v1.json", "ContactBook Contacts API V1");
    });

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        db.Database.Migrate(); // Ensures DB and schema exist
    }
}

app.Run();

