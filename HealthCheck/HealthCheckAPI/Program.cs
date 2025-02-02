using HealthCheckAPI;
using HealthCheckAPI.HealthCheck;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks()
  .AddCheck("ICMP_01", new ICMPHealthCheck("www.google.com", 100))
  .AddCheck("ICMP_02", new ICMPHealthCheck("www.ryadel.com", 100))
  .AddCheck("ICMP_03", new ICMPHealthCheck($"www.{Guid.NewGuid():N}.com", 100));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add a Cors Policy
builder.Services.AddCors(options => options.AddPolicy(name: "AngularPolicy", cfg => {
  cfg.AllowAnyHeader();
  cfg.AllowAnyMethod();
  cfg.WithOrigins(builder.Configuration["AllowedCORS"]);
}));

// Add SignalR service
builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// apply the CORS policy
app.UseCors("AngularPolicy");

app.UseHealthChecks(new PathString("/api/health"), new CustomHealthCheckOptions());

app.MapControllers();

// Offline Connection Test  
app.MapMethods("/api/heartbeat", new[] { "HEAD" }, () => Results.Ok());

// SignlR Hub
app.MapHub<HealthCheckHub>("/api/health-hub");
// SignlR broadcast message via minimal api instead of having a Controller
app.MapGet("/api/broadcast/update2", async (IHubContext<HealthCheckHub> hub) =>
{
  await hub.Clients.All.SendAsync("Update", "test");
  return Results.Text("Update2 message sent.");
});


app.MapFallbackToFile("/index.html");

app.Run();
