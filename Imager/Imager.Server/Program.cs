using Azure.Storage.Blobs;
using Imager.Server.Data;
using Imager.Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for Angular dev
builder.Services.AddCors(options => {
  options.AddPolicy("AllowAngular", policy =>
  {
    policy
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod();
  });
});

var blobConnectionString = builder.Configuration["AzureBlob:ConnectionString"];
builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));
builder.Services.AddScoped<AzureBlobStorageService>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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
