This file explains how Visual Studio created the project.

The following steps were used to generate this project:
- Create new ASP\.NET Core Web API project.
- Update `launchSettings.json` to register the SPA proxy as a startup assembly.
- Update project file to add a reference to the frontend project and set SPA properties.
- Add project to the startup projects list.
- Write this file.
- Remove `WeatherForecast.cs` file and anything related to Forecasting 
- Run `dotnet tool update --global dotnet-ef --version 8.023`
- Run `dotnet ef migrations add InitialCreate -o .\Data\Migrations`
- Run `dotnet ef database update`
- Run `dotnet ef migrations remove`

New EF Core Migration for Adding ImageId to ImageRecord
- Modify `ImageRecord.cs` Id will be internal identifier and the ImageId is the external id to expose
- Run `dotnet ef migrations add AddImageIdToImageRecord`
- Run `dotnet ef database update`
- Modified `ImagerAppController.cs` add ILogger<ImagerAppController> and changed GetImage {id} to be a GUID
- Modify `Program.cs` make sure that CORS is working fix http to https://localhost:4200

v1.0 Complete
- The .NET API is able to save the image within Azure Blob storage and fetch from the location.
  Along with saving the related meta data within a Azure SQL database using EF Core.

v1.1 Setup startup-time health valdation pipeline
- Uses Serilog for structured, production-grade telemetry
- Runs a set of internal health chekcs (Azure SQL, Azure Blob Storage, API handshake)
- Fails fast if any dependency is unhealthy
- Logs the failure in a structured. team-reviewable format
- Ensures Angular only talks to a health backend

- Install `Serilog.AspNetCore`
- Install `Serilog.Sinks.Console` Writes strucutred logs to the console (perfect for contains, app services)
- Install `Serilog.Sinks.ApplicationInsights` pushes structued logs into Azure Applicaiton Insights
- Install `Serilog.Sinks.File` writes rolling log files locally (useful ofr debugging)
- Install `Serilog.Settings.Configuration` allows Serilog to read from appsettings.json or serilog.json
- Install `Serilog.Extensions.Hosting` integrates Serilog into the .NET 8.0 generic host lifecycle
- Create file `serilog.json`
- Create file `HealthController.cs` for liveness and readiness endpoints for Azure and Kubernetes
- Create file `StartupHealthCheckService` for one-time startup validtion of main services