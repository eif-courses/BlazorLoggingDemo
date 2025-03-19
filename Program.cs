using BlazorLoggingDemo.Data;
using BlazorLoggingDemo.Repository;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var dbPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "logs.db");
// Ensure directory exists
Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

// Configure Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "BlazorLoggingDemo")
    .Enrich.WithMachineName()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.SQLite(
        sqliteDbPath: dbPath,
        tableName: "Logs",
        batchSize: 50));
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Services.AddScoped<ILogRepository, SqliteLogRepository>();
var app = builder.Build();
DatabaseInitializer.InitializeDatabase(
    builder.Configuration.GetConnectionString("LogsConnection"));
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
