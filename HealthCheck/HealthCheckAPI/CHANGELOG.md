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
- 