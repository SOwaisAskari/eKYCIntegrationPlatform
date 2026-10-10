using eKYC.Api.Data;
using eKYC.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DemoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Demo") ?? "Data Source=ekyc-demo.db"));
builder.Services.AddScoped<RequestService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
    db.Database.EnsureCreated();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", application = "eKYC Integration Platform Demo" }));
app.MapFallbackToFile("index.html");
app.Run();

public partial class Program;
