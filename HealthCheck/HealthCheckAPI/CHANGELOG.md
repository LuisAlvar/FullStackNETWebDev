This file explains how Visual Studio created the project.

The following steps were used to generate this project:
- Create new ASP\.NET Core Web API project.
- Update `launchSettings.json` to register the SPA proxy as a startup assembly.
- Update project file to add a reference to the frontend project and set SPA properties.
- Add project to the startup projects list.
- Write this file.

The following steps occurs after project was generated for our purpose:
- Update `launchSettings.json` change the iisSettings:iisExpress:applicationUrl port to 40080
- Update `launchSettings.json` change the iisSettings:iisExpress:sslPort to 40443
- Update `launchSettings.json` change profile HealthCheck.Server to HealthCheckAPI
- Update `launchSettings.json` set HealthCheckAPI:launchBrowser to false
- Update `launchSettings.json` hange the ports on HealthCheckAPI:applicationURL 
	- for https link set port to 40443
	- for http link set port to 40080
- Update `launchSettings.json` set IIS Express:launchBrowser to false
- Update `WeatherForecastController.cs` change the endpoint for this contoller to api/[controller]

- No longer need to add Nuget package Microsoft.AspNetcore.Diagnostics.HealthChecks
- Update `Program.cs` add HealthCheck to the service object
- Update `Program.cs` add UseHealthCheck and add /api/health as a param as type PathString to main WebApplication object
- Add new folder `./HealthCheck` and new file ICMPHealthCheck.cs
- Update `Program.cs` to HealthChecks() add AddCheck<ICMPHealthCheck>("ICMP")
- Update `ICMPHealthCheck.cs` add a constructor with params host and healthroundtriptime
	- Pass a custom message to the type of HealthCheckResult
- Update `Program.cs` add more AddChecks
- Add new file `CustomHealthCheckOptions.cs` under HealthCheck namespace for a more granular output with JSON structure. 
- Modify `Program.cs` add a minial api method for /api/heartbeat
- Modify `appsettings.json` add AllowedCORS: 
- Modify `Program.cs` add Cors Angular policy

02/01/2025
- Add Nuget Package `Microsoft.AspNetCore.SignalR` to HealthCheckAPI project
- Create a new file `HealthCheckHub.cs` at the root folder of HealthCheckAPI project it will inherit Hub
- Modify `Program.cs` setup service and middleware for SignlR
- Create a new controller `BroadcastController.cs` add service to be injected IHubContext<HealthCheckHub>
- Modify `Program.cs` add the SignlR middleware
- Modify `Projram.cs` missing middleware on app.UseCors("AngularPolicy");
- Modify `HealthCheckHub.cs` add a new method ClientUpdate to allow bi-directional data exchange
- Modify `Program.cs` add two middlewares for production: UseExceptionHandlerMiddleware and HSTSMiddleware
- Create a new file `web.config` this for IIS deployment and startup. We need to ensure this file gets passed when we build and publish the web service.
- Create a publish profile using Visual Studio 2022 UI within our local