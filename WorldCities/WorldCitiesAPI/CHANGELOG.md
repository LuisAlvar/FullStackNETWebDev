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
- Run `dotnet tool update --global dotnet-ef --version 6.0.36`
- Run `dotnet ef migrations add "Initial" -o "Data/Migrations"` for the first initial migration
- Run `dotnet ef database update` for apply a data migraiton 
	- Run `dotnet ef database drop` and `dotnet ef migrations remove` to drop the database and the migration and redo
- Remove functionality for `WeatherForecastController.cs` and `WeatherForecast.cs`
- Add new controller base `SeedController.cs`
- Add NuGet package `EPPlus`  search for version 4.5.3.3
- Add `CitiesController.cs`
- Add `CountriesController.cs`
- Modify `Program.cs` add JsonOptions to the AddController set JsonSerializerOptions.WeriteIndented to true
- Modify `CitiesController.cs` GetCities method to implement a paging type functionality against the list of cities. 
- Create new `Data/ApiResult.cs` file with pageIndex and pageSize
- Modify `ApiResult.cs` add sorting functionality
- Modify `CitiesController.cs` update GetCities with sorting functionality.
- Modify `Data/Country.cs` add JsonPropertyName("iso3") for iso2 and iso3
- Modify `CitiesController.cs` add isDupeCity functionality
- Create `Data\CountryDTO.cs` with all of the properties of Country plus a interger count on Cities
- Modify `CountriesControlller.cs` change the getcountries to return ApiResult<CountryDTO>
- Create `Data\CityDTO.cs` file 
- Modify `CitiesController.cs` change the getcities to return ApiResult<CityDTO>

01/04/2025
- Add Nuget package `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
- Add Nuget package `Microsoft.AspNetCore.Authentication.JwtBearer`
- Create `Data/Model/ApplicationUser.cs` inherit IdentityUser
- Modify `ApplicationDbContext.cs` extended from a different database abstraction base class that supports ASP.NET Core Identity.
	- Change DbContext to IdentityDbContext<ApplicationUser>
- Modify `Program.cs` file 
- Create `Data/LoginRequest.cs` DTO class to hold a user creditentials
- Create `Data/LoginResult.cs` DTO class to return id data to user 
- Modify `appsettings.json` adding JWT settings 
- Create `Data/JwtHandler.cs`
- Modify `Program.cs` add service to JwtHandler as a Scope service
- Create `Controllers/AccountController.cs` 
- Modify `Program.cs` properly set up the JwtBearerMiddleware
- Modify `SeedController.cs` and Identity features, implementation of CreateDefaultUsers and added the Authorized attribute to the class or methods.
- Run `dotnet ef migrations add "Identity" -o "Data/Migrations"`
- Run `dotnet ef database update`
	- options to drop database again run `dotnet ef database drop`
  - options to database update `dotnet ef database update`
- Modify `appsettings.json` make sure to have JwtSetting:SecurityKey to contain a GUID or string with more than 256 bytes
- Create `RegisterRequest.cs` contain the same as LoginRequest but with the added username
- Create `RegisterResult.cs` similar to loginresult
- Modify `AccountController.cs` add a new httppost for Register() and Id = Guid.NewGuid().ToString() to ApplicationUser
- Modify `SeedController.cs` file add Id = Guid.NewGuid().ToString() to ApplicationUser

- Modify `AccountController.cs` convert the ProtectedEndpoint method to a minimual api method within `Program.cs`
- Modify `Program.cs` add a minimal api method for /api/heartbeat


01/31/2025
- Add Nuget package `HotChocolate.AspNetCore --version 14.3.0`
- Add Nuget package `HotChocolate.AspNetcore.Authorization --verizon 14.3.0`
- Add Nuget package `HotChocolate.Data.EntityFramework --version 14.3.0`
- Update Nuget package `System.Linq.Dynamic.Core --version 1.6.0`
- Create a new folder	`/Data/GraphQL`
- Create a new file `Query.cs` under `/Data/GraphQL`
- Create a new file	`Mutation.cs` under `/Data/GraphQL`
- Modify `Program.cs` add to Services.AddGraphQL 
	- and app.MapGraphQL for HotChocoloate built-inGraphQL web-based client
- Modify `Query.cs` add a new method GetCitiesApiResult
- Modify `Program.cs` add two middlewares for production: UseExceptionHandlerMiddleware and HSTSMiddleware
