import { Component , OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { City } from './city';
import { Country } from '../countries/country';
import { environment } from './../../envrionments/environment';

/*
Utilized component for both editing city data and creating new city data
*/

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrl: './city-edit.component.scss'
})
export class CityEditComponent implements OnInit{
  // the view title
  title?: string;

  // the form model
  form!: FormGroup;

  // the city object to edit
  city?: City;

  /*
  the city object id, as fetched from the active route:
  It's NULL when we're adding a new city,
  and not NULL when we're editing an existing one.
  */
  id?: number;

  // the countries array for the select
  countries?: Country[];

  constructor(private activatedRoute: ActivatedRoute, private router: Router, private http: HttpClient) { }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl(''),
      lat: new FormControl(''),
      lon: new FormControl(''),
      countryId: new FormControl('')
    });
    this.loadData();
  }

  loadData() {
    // load countries
    this.loadCountries();

    // retrieve the ID from the 'id' paramter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDITING MODE

      // fetch the city from the server
      var url = environment.baseURL + 'api/Cities/' + this.id;
      this.http.get<City>(url).subscribe(result => {
        this.city = result;
        this.title = "Edit - " + this.city.name;

        //update the form with the city value
        this.form.patchValue(this.city);
      }, error => console.error(error));
    }
    else {
      //ADD NEW DATA MODE
      this.title = "Create a new City";
    }
  } // end of loadData

  loadCountries() {
    // fetch all the countires form the server
    var url = environment.baseURL + 'api/Countries';
    var params = new HttpParams()
      .set("pageIndex", "0")
      .set("pageSize", "9999")
      .set("sortColumn", "name")
      .set("sortOrder", "asc");

    this.http.get<any>(url, { params })
      .subscribe(result => {
        this.countries = result.data;
    }, error => console.error(error));
  }

  onSubmit() {
    var city = (this.id) ? this.city : <City>{};
    if (city) {
      city.name = this.form.controls['name'].value;
      city.lat = +this.form.controls['lat'].value;
      city.lon = +this.form.controls['lon'].value;
      city.countryId = +this.form.controls['countryId'].value;

      if (this.id) {
        // EDIT MODE
        var url = environment.baseURL + 'api/Cities/' + city.id;
        this.http.put<City>(url, city).subscribe(result => {
          console.log("City " + city!.id + " has been updated.");

          // go back to cities view
          this.router.navigate(['/cities']);
        }, error => console.error(error));
      }
      else {
        // ADD NEW MODE
        var url = environment.baseURL + 'api/Cities';
        console.log(city);
        this.http.post<City>(url, city).subscribe(result => {
          console.log("City " + city!.id + " has been created.");
          // go back to cities view
          this.router.navigate(['/cities']);
        }, error => console.error(error));
      }
    }
  } // end of OnSubmit



}
