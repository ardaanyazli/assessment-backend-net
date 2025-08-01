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
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();
        db.Database.Migrate(); // Ensures DB and schema exist
    }
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseHealthChecks("/health");
app.Run();

