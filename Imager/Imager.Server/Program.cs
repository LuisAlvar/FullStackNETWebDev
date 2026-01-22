using Azure.Storage.Blobs;
using Imager.Server.Data;
using Imager.Server.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// EF Core 
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Azure SQL Server: Endpoint: {sqlConnectionString}");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlConnectionString));

// Blob Client
var blobConnectionString = builder.Configuration["AzureBlob:ConnectionString"];
Console.WriteLine($"Azure Blob Storage: Endpoint: {blobConnectionString}");
builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));
builder.Services.AddSingleton<AzureBlobStorageService>();

// Startup health service
builder.Services.AddSingleton<StartupHealthCheckService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Angular dev
builder.Services.AddCors(options => {
  options.AddPolicy("AllowAngular", policy =>
  {
    policy
    .WithOrigins("https://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod();
  });
});

var app = builder.Build();

#region RUN STARTUP HEALTH CHECKS (ONE-TIME VALIDATION)
using (var scope = app.Services.CreateScope())
{
  var health = scope.ServiceProvider.GetRequiredService<StartupHealthCheckService>();
  try
  {
    Log.Information("Running startup health checks ...");
    await health.ValidateAsync();
    Log.Information("All startup health checks passed");
  }
  catch (Exception ex)
  {
    Log.Fatal(ex, "Startup health checks FAILED - shutting down");
    throw;
  }
}
#endregion

#region  APPLY EF Core MIGRATIONS ONLY IN PRODUCTION
if (app.Environment.IsProduction())
{
  using var scope = app.Services.CreateScope();
  var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  try
  {
    logger.LogInformation("Apply EF Core migraitons in Production ...");
    db.Database.Migrate();
    logger.LogInformation("EF Core migraitons applied successfully.");
  }
  catch (Exception ex)
  {
    logger.LogError(ex, "Error applying EF Core migrations.");
    throw;
  }
}  
#endregion


app.UseCors("AllowAngular");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseStaticFiles(); // for wwwroot/uploads
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");
app.Run();
