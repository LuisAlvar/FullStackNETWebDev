import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from './../../envrionments/environment';
import { Country } from './country';
import { BaseFormComponent } from '../base-form.component';


@Component({
  selector: 'app-country-edit',
  templateUrl: './country-edit.component.html',
  styleUrl: './country-edit.component.scss'
})
export class CountryEditComponent extends BaseFormComponent implements OnInit{
  // the view title
  title?: string;

  // the country object to edit to create
  country?: Country;

  /*
  the country object id, as fetched from the active route:
  It's NULL when we're adding a new country,
  and not NULL when we're editing an existing one. 
  */
  id?: number;

  // the countries array for the select
  countries?: Country[];

  // In the case where a propery value is dupe, then we should not allow the user to submit data
  aProperyValueIsDupe: Boolean = false;

  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient)
  {
    super();    
  }

  ngOnInit() {
    this.form = this.fb.group({
      name: ['', Validators.required, this.isDupeField("name")],
      iso2: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{2}$/)], this.isDupeField("iso2")],
      iso3: ['', [Validators.required, Validators.pattern(/^[a-zA-Z]{3}$/)], this.isDupeField("iso3")]
    });
    this.loadData();
  }

  loadData() {
    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE

      // fetch the country from the server
      var url = environment.baseURL + 'api/Countries/' + this.id;
      this.http.get<Country>(url).subscribe(result => {
        this.country = result;
        this.title = "Edit - " + this.country.name;

        //update the form with the country value
        this.form.patchValue(this.country);
      }, error => console.error(error));
    }
    else {
      // ADD NEW MODE
      this.title = "Create a new Country";
    }
  }

  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};
    if (country && !this.aProperyValueIsDupe) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;
      if (this.id) {
        //EDIT MODE
        var url = environment.baseURL + 'api/Countries/' + this.id;
        this.http.put<Country>(url, country).subscribe(result => {
          console.log("Country " + country!.id + " has been updated.");
          // go back to countries view
          this.router.navigate(['/countries']);
        }, error => console.error(error));
      }
      else {
        // ADD NEW MODE
        var url = environment.baseURL + 'api/Countries';
        this.http.post<Country>(url, country).subscribe(result => {
          console.log("Country " + result.id + " has been created.");
          // go back to countries view
          this.router.navigate(['/countries']);
        }, error => console.error(error));
      }
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{[key: string]: any } | null> => {
      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value);

      var url = environment.baseURL + 'api/Countries/IsDupeField';
      return this.http.post<boolean>(url, null, { params })
        .pipe(map(result => {
          this.aProperyValueIsDupe = result;
          return (result ? { isDupeField: true } : null);
        }));
    }
  }


}
