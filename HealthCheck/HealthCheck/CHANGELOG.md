This file explains how Visual Studio created the project.

The following tools were used to generate this project:
- Angular CLI (ng)

The following steps were used to generate this project:
- Create Angular project with ng: `ng new healthcheck.client --defaults --skip-install --skip-git --no-standalone `.
- Add `proxy.conf.js` to proxy calls to the backend ASP.NET server.
- Add `aspnetcore-https.js` script to install https certs.
- Update `package.json` to call `aspnetcore-https.js` and serve with https.
- Update `angular.json` to point to `proxy.conf.js`.
- Update `app.component.ts` component to fetch and display weather information.
- Modify `app.component.spec.ts` with updated tests.
- Update `app.module.ts` to import the HttpClientModule.
- Create project file (`healthcheck.client.esproj`).
- Create `launch.json` to enable debugging.
- Create `nuget.config` to specify location of the JavaScript Project System SDK (which is used in the first line in `healthcheck.client.esproj`).
- Update package.json to add `jest-editor-support`.
- Update package.json to add `run-script-os`.
- Add `karma.conf.js` for unit tests.
- Update `angular.json` to point to `karma.conf.js`.
- Add project to solution.
- Add project to the startup projects list.
- Write this file.

The following steps occurs after project was generated for our purpose:
- Modify `proxy.conf.js` with target value else to https://localhost:40443
- Modify `launch.json` switch the array placement with edge and chrome. 
- Update `package.json` any instance of healthcheck.client change to HealthCheck
- Update `angular.json` any instance of healthcheck.client change the HealthCheck
- Update `app.component.ts` change the title healthcheck.client to HealthCheck

- Update `proxy.conf.js` the context array type with "/api" as the single element within the array
- Update `app.component.ts` add api as the prefix to the weatherforecast endpoint /api/weatherforecast. 
- Update `app.component.spec.ts` test case for 'shouldretrieve weather forecast from the server' change the endpoint to /api/weatherforecast 

- Add new folder under `/src` called environments
- Add file `enviroment.ts` for debug/dev 
- Add file `enviroment.prod.ts` for production

- Run `ng generate component Home --skip-tests`
- Run `ng generate component FetchData --skip-tests`
- Run `ng generate component NavMenu --skip-tests`
- Move the get weatherforecast logic from app.component to fetchdata.component
- Modify `app.component.html` to contain the app-nav-menu and router-outlet tags. 
- Modify `nav-menu.component.html` for some odd reason html browsers dont like two individual anchor tags together one after the other. 
  - Placed the anchor within their own list type element 
- Modify `home.component.html` contains a simple greeting. 

- Run `ng generate compontn HealthCheck --module=app --skip-tests` and add all of functionality to call HealthCheckAPI /api/health and display the information
- Modify `nav-menu.component.html` add a new anchor tag for Health Check
- Modify `app-routing.module.ts` add new route for 'health-check' for the HealthCheckComponent
- Modify `app.module.ts` add HealthCheckComponent

- Run `ng add @angular/material`
- Modify `app.module.ts` add MatButtonModule, MatIconModule, MatToolbarModule
- Update `nav-menu.component.html`
- Error with @angular/material 
- remove node_modules and package-lock.json
- Ran `npm cache clean --force`
- Ran `npm install`
- Ran `ng add @angular/material`_
- Modify `app.component.css` add padding 
- Modify `angular.json` switch from css to scss; there are two location that require change; 
  - will need to change all .css files to scss file extensions
  - along with modifying all of the ts file with new style file extension
- Modify `styles.scss` redesign table 
