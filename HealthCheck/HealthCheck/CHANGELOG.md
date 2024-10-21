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
