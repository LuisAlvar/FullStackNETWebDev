using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using WorldCitiesAPI.Data.GraphQL;

var builder = WebApplication.CreateBuilder(args);



// Adding Serilog support
if (!builder.Environment.IsEnvironment("Testing"))
{
  builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.MSSqlServer(
      connectionString: ctx.Configuration.GetConnectionString("DefaultConnection"),
      restrictedToMinimumLevel: LogEventLevel.Information,
      sinkOptions: new MSSqlServerSinkOptions
      {
        TableName = "LogEvents",
        AutoCreateSqlTable = true,
      })
    .WriteTo.Console()
  );
}


builder.Services.AddControllers().AddJsonOptions(options => {
  options.JsonSerializerOptions.WriteIndented = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add ApplicationDbContext and SQL Server support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!.ToString()));


// Add ASP.NET Core Identity support 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
  options.SignIn.RequireConfirmedAccount = true;
  options.Password.RequireDigit = true;
  options.Password.RequireLowercase = true;
  options.Password.RequireUppercase = true;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequiredLength = 8;

}).AddEntityFrameworkStores<ApplicationDbContext>();

// Add Our Jwt Token Generation Handler
builder.Services.AddScoped<JwtHandler>();

// Add GraphQL via HotChocoloate
builder.Services.AddGraphQLServer()
  .AddAuthorization()
  .AddQueryType<Query>()
  .AddMutationType<Mutation>()
  .AddFiltering()
  .AddSorting();

// Configure JWT Authentication 
builder.Services.AddAuthentication(opt =>
{
  opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    RequireExpirationTime = true,
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
    ValidAudience = builder.Configuration["JwtSettings:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecurityKey"]))
  };

  options.Events = new JwtBearerEvents
  {
    OnMessageReceived = context =>
    {
      context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
      if (!string.IsNullOrEmpty(refreshToken))
      {
        context.Token = refreshToken;
      }
      return Task.CompletedTask;
    }
  };
});


if (!builder.Environment.IsProduction())
{
  Debug.WriteLine("Configuration Settings:");
  foreach (var kvp in builder.Configuration.AsEnumerable())
  {
    Debug.WriteLine($"{kvp.Key}: {kvp.Value}");
  }
}

var app = builder.Build();


if (!builder.Environment.IsEnvironment("Testing"))
{
  // adding logging HTTP request middleware support
  app.UseSerilogRequestLogging();
}

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL("/api/graphql");

app.MapGet("/api/Account/ProtectedEndpoint", () => {
  return Results.Ok("You have accessed a protected endpoint.");
}).RequireAuthorization();

app.MapMethods("/api/heartbeat", new[] { "HEAD" }, () => Results.Ok());

app.MapFallbackToFile("/index.html");

app.Run();

public partial class Program { }