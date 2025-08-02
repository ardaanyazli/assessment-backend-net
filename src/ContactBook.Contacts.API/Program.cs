using ContactBook.Contacts.API.Middleware;
using ContactBook.Contacts.Application.Interfaces;
using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Contacts.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ContactsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("contactsDb")));
builder.Services.AddScoped<IContactsUnitOfWork, ContactsUnitOfWork>();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var cancellationToken = context.RequestAborted;

    try
    {
        // Simulate artificial latency (e.g., 2 seconds)
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
    catch (OperationCanceledException)
    {
        // If request was cancelled during the delay, let RequestCancellationMiddleware handle it
        return;
    }

    await next(); // Continue to next middleware
});
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestCancellationMiddleware>();
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

