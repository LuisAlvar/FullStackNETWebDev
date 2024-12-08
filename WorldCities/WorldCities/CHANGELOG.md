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