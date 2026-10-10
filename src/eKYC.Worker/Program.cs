using eKYC.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection("Worker"));
builder.Services.AddHttpClient("eKYC.Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Worker:ApiBaseUrl"] ?? "http://localhost:5188/");
    client.Timeout = TimeSpan.FromSeconds(20);
});
builder.Services.AddHostedService<KycProcessingWorker>();
await builder.Build().RunAsync();
