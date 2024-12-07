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

12/05/2024
- Add NuGet packages `Microsoft.EntityframeworkCore` latest version for .NET 6 is 6.0.36
- Add NuGet packages `Microsoft.EntityFrameworkCore.Tools` latest version for .NET 6 is 6.0.36
- Add NuGet packages `Microsoft.EntityFrameworkCore.SqlServer` latest version for .NET 6 is 6.0.36

- 12/06/2024
- Create a new folder at rool level `Data`
- Create a new folder `Data\Models`
- Create a new file within `City.cs` at `Data\Models`
- Create a new file within `Country.cs` at `Data\Models`
- Create a new file within `ApplicationDbContext` at `Data`
- Manage user secrets for this project. 
- Add SQL Server connection string to	`secrets.json`
- Modify `Program.cs` file add ApplicationDbContext and SQL Server support