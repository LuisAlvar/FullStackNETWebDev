This file explains how Visual Studio created the project.

The following steps were used to generate this project:
- Create new ASP\.NET Core Web API project.
- Update `launchSettings.json` to register the SPA proxy as a startup assembly.
- Update project file to add a reference to the frontend project and set SPA properties.
- Add project to the startup projects list.
- Write this file.
- Change the Project name from WorldCities.server to WorldCitiesAPI
- Change the namespace to WorldCitiesAPI
- Change the folder as well 
- Modify `launchSettings.json` 
	- set launchBrowser to false
	- IISExpress URL and port to 40080
	- URL https://localhost/40443 and http://localhost:40080
