using ContactBook.Reports.Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ReportWorker>();

var host = builder.Build();
host.Run();
