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

Future Migraiton 
- Run `dotnet ef migrations add AddTagsToImages`
- Run `dotnet ef database update`


