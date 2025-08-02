using ContactBook.Contacts.Infrastructure.Persistence;
using ContactBook.Reports.Consumer;
using ContactBook.Reports.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<ReportDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("reportsDb")));
builder.Services.AddDbContext<ContactsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("contactsDb")));
builder.Services.AddHostedService<ReportWorker>();

var host = builder.Build();
host.Run();
