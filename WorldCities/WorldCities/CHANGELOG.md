This file explains how Visual Studio created the project.

The following tools were used to generate this project:
- Angular CLI (ng)

The following steps were used to generate this project:
- Create Angular project with ng: `ng new worldcities.client --defaults --skip-install --skip-git --no-standalone `.
- Add `proxy.conf.js` to proxy calls to the backend ASP.NET server.
- Add `aspnetcore-https.js` script to install https certs.
- Update `package.json` to call `aspnetcore-https.js` and serve with https.
- Update `angular.json` to point to `proxy.conf.js`.
- Update `app.component.ts` component to fetch and display weather information.
- Modify `app.component.spec.ts` with updated tests.
- Update `app.module.ts` to import the HttpClientModule.
- Create project file (`worldcities.client.esproj`).
- Create `launch.json` to enable debugging.
- Create `nuget.config` to specify location of the JavaScript Project System SDK (which is used in the first line in `worldcities.client.esproj`).
- Update package.json to add `jest-editor-support`.
- Update package.json to add `run-script-os`.
- Add `karma.conf.js` for unit tests.
- Update `angular.json` to point to `karma.conf.js`.
- Add project to solution.
- Add project to the startup projects list.
- Write this file.
12/3/2024
- Change project name from worldcities.client to WorldCities
- Modify `package.json` change name to WorldCities
- Modify `angular.json` change output path to dist/WorldCities also under configurations for envirnoments WorldCities:build:developement
- Modify `proxy.conf.js` changes the context to just /api and changed the port to 40443
12/4/2024
- Run `ng generate componet Home --skip-tests --module=app`
- Run `ng generate component NavMenu --skip-tests --module=app`
- Remove forcast weather functionality.
- Edit `app.component.html` to only contain the router-outlet tag and the app-nav-menu tag
- Edit `app-routing.module.ts` add path = '' for HomeComponent
- Create a new folder /src/envirnoments
- Add envirnoments.ts and envirnoments.prod.ts files under the new folder created above.
- Edit `environment.ts` add export const with production and baseURL properties with baseURL set to /
- Edit `environment.prod.ts`  add export const with production and baseURL properties with baseURL set to https://localhost:40443
- Run `ng add @angular/material` - need to remove node_modules and package-lock.json, npm cache clean -force, npm install, re-run command.
12/05/2024
- Replace all css files with scss file and modify all ts files with css reference
- Add new image to `public` folder
- Modify `home.component.html` to a header, paragraph, and image tag
12/08/2024
- Run `ng generate component Cities --moduel=app --skip-tests`
- Modify `nav-menu.component.html` add anchor tag for cities 
- Modify `app-routing.module.ts`  add path for citiescomponent
- Create `angular-material.module.ts` file for all of the angualr material imported modules
- Update `cities.component.html` with new angular material table
- Update `cities.component.ts` 
- Modify `angular-material.module.ts`  import MatPaginatorModule
- Modify `cities.component.ts` add MatTableDataSource, MatPaginator, and ViewChild
- Modify `cities.component.html` append the mat-paginator directive
- Modify `cities.component.ts` adding logic on http param
- Update `angular-material.module.ts` add MatSort
- Update `cities.component.ts` implement sorting business logic
- Update `cities.component.html` with mat-form-field and use MatInputModule
- Run `ng generate component Countries --skip-tests --module=app`
- New file `countries\country.ts` for country type data
- Modify `angular-material.moduel.ts` add ReactiveFormsModule from @angular/forms
- With `app\cities` run `ng generate component CityEdit --skip-tests --module=app --flat`
- Modify `app-routing.module.ts` add the routing rule for 'city:/id'
- Modify `cities.component.html` create a routerLink
- Modify `angular-material.module.ts` add FormsModuel and ReactiveFormsModule
- Modify `city-edit.component.ts` to add dual functionality for editing and creating a new city
- Modify `app-routing.module.ts`  two different routing rules exist for CityEditComponent
- Update `city.cs` file add countryId: number; 
- Modify `angular-material.module.ts` add MatSelectModule @angular/material/select
- Modify `city-edit.component.ts` add Validators from @angular/forms
- Modify `city-edit.component.html` add <mat-error>
- Modify `city-edit.component.ts` add custom validator for async call to back-end
- Run `ng generate component CountryEdit --flat --module=app --skip-tests` within `src/app/countries`
- Modify `app-routing.module.ts` to contain new routing path for country/:id and country/ for CountryEditComponent
- Modify `countries.component.html` add button for add new city and add anchor tag to country.name to CountryEditComponet
