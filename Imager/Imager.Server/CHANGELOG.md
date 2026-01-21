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