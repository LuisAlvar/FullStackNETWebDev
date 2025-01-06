- Run `dotnet new xunit -o WorldCitiesAPUI.Tests`
- Add Nuget package `Moq --version 4.20.72`
- Add Nuget package `Microsoft.EntityFrameworkCore.InMemory --version 6.0.36`
- Add WorldCitiesAPI project as a dependences to WorldCitiesAPI.Tests
- Create `CitiesController_Test.cs` will hold all of the unit test cases for the cities controller.cs
- Create `IdentityHelper.cs` this contains helping logic with Microsoft.AspNetCore.Authorization to generate token logic for the test
- Create `SeendController_Test.cs` mainly contians the logic for test for CreateDefaultUsers with the SeedController



- 