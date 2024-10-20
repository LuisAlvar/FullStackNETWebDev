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
